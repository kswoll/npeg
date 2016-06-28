using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using PEG.Proxies.Extensions;

namespace PEG.Proxies
{
    /// <summary>
    /// Generates proxies for the following usage scenarios:<ul>
    ///
    /// <li>Create a proxy on a base class where the proxy class should override virtual methods
    ///     in the base class, making those methods available to the proxy.  In this context,
    ///     Invocation.Proceed invokes the base method implementation.</li>
    ///
    /// <li>Create a proxy on an interface while supplying a target implementation of that
    ///     interface.  In this context, Invocation.Proceed invokes the method on the provided
    ///     target.</li>
    ///
    /// <li>Create a proxy on an interface, not providing any target.  In this context,
    ///     Invocation.Proceed does nothing.</li>
    ///
    /// </ul>
    ///
    /// <b>Note:</b> Generated implementations are stored in a static field of the generic
    /// Proxy&lt;T&gt; class.  These are instantiated upon first access of a particular
    /// variant of that class (variant on the type argument), which solves any thread
    /// contention issues.
    /// </summary>
    public class Proxy
    {
        /// <summary>
        /// Creates a proxy for a given type.  This method supports two discrete usage scenarios.<p/>
        /// If T is an interface, the target should be an implementation of that interface. In
        /// this scenario, T should be <i>explicitly</i> specified unless the type of <i>target</i>
        /// at the calling site is of that interface.  In other words, if the calling site has the
        /// <i>target</i> declared as the concrete implementation, the proxy will be generated
        /// for the implementation, rather than for the interface.
        ///
        /// If T is a class, the target should be an instance of that class, and a subclassing
        /// proxy will be created for it.  However, because target is specified in this case,
        /// the base class behavior will be ignored (it will all be delegated to the target).
        /// </summary>
        /// <typeparam name="T">The type to create the proxy for.  May be an interface or a
        /// concrete base class.</typeparam>
        /// <param name="target">The instance of T that should be the recipient of all invocations
        /// on the proxy via Invocation.Proceed.</param>
        /// <param name="invocationHandler">This is where you get to inject your logic.</param>
        /// <returns>The new instance of the proxy that is an instance of T</returns>
        public static T CreateProxy<T>(T target, Action<Invocation> invocationHandler)
        {
            return Proxy<T>.CreateProxy(target, invocationHandler);
        }

        /// <summary>
        /// Creates a proxy for a given type.  This overload does not accept a target.  If T is
        /// an interface, then calls to Proceed will do nothing.  If T is a class, calls to Proceed
        /// will invoke the base class behavior.
        /// </summary>
        /// <typeparam name="T">The type to create the proxy for.  May be an interface or a
        /// concrete base class.</typeparam>
        /// <param name="invocationHandler"></param>
        /// <returns>The new instance of the proxy that is an instance of T</returns>
        public static T CreateProxy<T>(Action<Invocation> invocationHandler)
        {
            return Proxy<T>.CreateProxy(invocationHandler);
        }
    }

    public class Proxy<T>
    {
        private static ConstructorInfo invocationConstructor = typeof(Invocation).GetConstructors()[0];
        private static MethodInfo invokeMethod = typeof(Action<Invocation>).GetMethod("Invoke");
        private static MethodInfo getReturnValue = typeof(Invocation).GetProperty("ReturnValue").GetGetMethod();

        private static Type proxyType = CreateProxyType();

        public static T CreateProxy(T target, Action<Invocation> invocationHandler)
        {
            return (T)Activator.CreateInstance(proxyType, target, invocationHandler);
        }

        public static T CreateProxy(Action<Invocation> invocationHandler)
        {
            return (T)Activator.CreateInstance(proxyType, invocationHandler);
        }

        private static Type CreateProxyType()
        {
            string assemblyName = typeof(T).FullName + "__Proxy";

            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder module = assembly.DefineDynamicModule(assemblyName, @"temp.module.dll");
            bool isIntf = typeof(T).IsInterface;

            Type baseType = isIntf ? typeof(object) : typeof(T);
            Type[] intfs = isIntf ? new[] { typeof(T) } : Type.EmptyTypes;

            var type = module.DefineType(assemblyName, TypeAttributes.Public, baseType, intfs);

            // Create target field
            var target = type.DefineField("__target", typeof(T), FieldAttributes.Private);
            var invocationHandler = type.DefineField("__invocationHandler", typeof(Action<Invocation>), FieldAttributes.Private);

            // Create default constructor
            var constructorWithoutTarget = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(Action<Invocation>) });
            var constructorWithoutTargetIl = constructorWithoutTarget.GetILGenerator();
            constructorWithoutTargetIl.EmitDefaultBaseConstructorCall(typeof(T));
            constructorWithoutTargetIl.Emit(OpCodes.Ldarg_0);
            constructorWithoutTargetIl.Emit(OpCodes.Ldarg_1);
            constructorWithoutTargetIl.Emit(OpCodes.Stfld, invocationHandler);
            constructorWithoutTargetIl.Emit(OpCodes.Ret);

            // Create constructor with target
            var constructorWithTarget = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(T), typeof(Action<Invocation>) });
            var constructorWithTargetIl = constructorWithTarget.GetILGenerator();
            constructorWithTargetIl.EmitDefaultBaseConstructorCall(typeof(T));
            constructorWithTargetIl.Emit(OpCodes.Ldarg_0);
            constructorWithTargetIl.Emit(OpCodes.Ldarg_1);
            constructorWithTargetIl.Emit(OpCodes.Stfld, target);
            constructorWithTargetIl.Emit(OpCodes.Ldarg_0);
            constructorWithTargetIl.Emit(OpCodes.Ldarg_2);
            constructorWithTargetIl.Emit(OpCodes.Stfld, invocationHandler);
            constructorWithTargetIl.Emit(OpCodes.Ret);

            var staticConstructor = type.DefineConstructor(MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Static, CallingConventions.Standard, Type.EmptyTypes);
            var staticIl = staticConstructor.GetILGenerator();

            // Now implement/override all methods
            foreach (var methodInfo in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                ParameterInfo[] parameterInfos = methodInfo.GetParameters();

                // Finalize doesn't work if we try to proxy it and really, who cares?
                if (methodInfo.Name == "Finalize" && parameterInfos.Length == 0 && methodInfo.DeclaringType == typeof(object))
                    continue;
                if (methodInfo.Name == "OnCreated")
                    continue;

                if (isIntf || methodInfo.IsVirtual)
                {
                    MethodAttributes methodAttributes;
                    if (isIntf)
                        methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;
                    else
                    {
                        methodAttributes = methodInfo.IsPublic ? MethodAttributes.Public : MethodAttributes.Family;
                        methodAttributes |= MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual;
                    }

                    MethodBuilder method = type.DefineMethod(methodInfo.Name, methodAttributes, methodInfo.ReturnType, parameterInfos.Select(x => x.ParameterType).ToArray());

                    // Initialize method info in static constructor
                    var methodInfoField = type.DefineField(methodInfo.Name + "__Info", typeof(MethodInfo), FieldAttributes.Private | FieldAttributes.Static);
                    staticIl.StoreMethodInfo(methodInfoField, methodInfo);

                    // Create proceed method
                    var proceed = type.DefineMethod(methodInfo.Name + "__Proceed", MethodAttributes.Private, typeof(object), new[] { typeof(object[]) });
                    ILGenerator proceedIl = proceed.GetILGenerator();

                    // Load target for subsequent call
                    proceedIl.Emit(OpCodes.Ldarg_0);
                    proceedIl.Emit(OpCodes.Ldfld, target);
                    proceedIl.Emit(OpCodes.Dup);

                    var targetNotNull = proceedIl.DefineLabel();
//                    var returnFromMethod = proceedIl.DefineLabel();

                    // If target is null, we will do a base method invocation, if possible
                    proceedIl.Emit(OpCodes.Brtrue, targetNotNull);

                    // Pop the null target off the stack
                    proceedIl.Emit(OpCodes.Pop);

                    if (!isIntf)
                    {
                        // Target is null and we are overriding a base type, call the base implementation
                        proceedIl.Emit(OpCodes.Ldarg_0);
                    }
                    else
                    {
                        if (method.ReturnType != typeof(void))
                        {
                            if (methodInfo.ReturnType.IsValueType)
                            {
                                proceedIl.EmitDefaultValue(methodInfo.ReturnType);
                            }
                            else
                            {
                                proceedIl.Emit(OpCodes.Ldnull);
                            }
                        }
                        proceedIl.Emit(OpCodes.Ret);
                    }

                    proceedIl.MarkLabel(targetNotNull);

                    // Decompose array into arguments
                    for (int i = 0; i < parameterInfos.Length; i++)
                    {
                        proceedIl.Emit(OpCodes.Ldarg, (short)1);            // Push array
                        proceedIl.Emit(OpCodes.Ldc_I4, i);                  // Push element index
                        proceedIl.Emit(OpCodes.Ldelem, typeof(object));     // Get element
                        if (parameterInfos[i].ParameterType.IsValueType)
                            proceedIl.Emit(OpCodes.Unbox_Any, parameterInfos[i].ParameterType);
                        else
                            proceedIl.Emit(OpCodes.Castclass, parameterInfos[i].ParameterType);
                    }

                    proceedIl.Emit(isIntf ? OpCodes.Callvirt : OpCodes.Call, methodInfo);
                    if (methodInfo.ReturnType.IsValueType && methodInfo.ReturnType != typeof(void))
                        proceedIl.Emit(OpCodes.Box, method.ReturnType);
                    proceedIl.Emit(OpCodes.Ret);

                    // Implement method
                    ILGenerator il = method.GetILGenerator();

                    // Call handler
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, invocationHandler);

                    // Load method info
                    il.Emit(OpCodes.Ldsfld, methodInfoField);

                    // Create arguments array
                    il.Emit(OpCodes.Ldc_I4, parameterInfos.Length);         // Array length
                    il.Emit(OpCodes.Newarr, typeof(object));                // Instantiate array
                    for (int i = 0; i < parameterInfos.Length; i++)
                    {
                        il.Emit(OpCodes.Dup);                               // Duplicate array
                        il.Emit(OpCodes.Ldc_I4, i);                         // Array index
                        il.Emit(OpCodes.Ldarg, (short)(i + 1));             // Element value

                        if (parameterInfos[i].ParameterType.IsValueType)
                            il.Emit(OpCodes.Box, parameterInfos[i].ParameterType);

                        il.Emit(OpCodes.Stelem, typeof(object));            // Set array at index to element value
                    }

                    // Load function pointer to proceed method
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldftn, proceed);
                    il.Emit(OpCodes.Newobj, typeof(Func<object[], object>).GetConstructors()[0]);

                    // Instantiate Invocation
                    il.Emit(OpCodes.Newobj, invocationConstructor);

                    // Store invocation
                    il.Emit(OpCodes.Dup);
                    var invocationVariable = il.DeclareLocal(typeof(Invocation));
                    il.Emit(OpCodes.Stloc, invocationVariable);

                    // Invoke handler
                    il.Emit(OpCodes.Callvirt, invokeMethod);

                    // Extract return value
                    if (methodInfo.ReturnType != typeof(void))
                    {
                        il.Emit(OpCodes.Ldloc, invocationVariable);
                        il.Emit(OpCodes.Call, getReturnValue);

                        if (methodInfo.ReturnType.IsValueType)
                        {
                            var afterLoadDefaultValue = il.DefineLabel();
                            var afterUnbox = il.DefineLabel();

                            il.Emit(OpCodes.Brtrue, afterLoadDefaultValue);
                            il.EmitDefaultValue(methodInfo.ReturnType);
                            il.Emit(OpCodes.Br, afterUnbox);
                            il.MarkLabel(afterLoadDefaultValue);
                            il.Emit(OpCodes.Ldloc, invocationVariable);
                            il.Emit(OpCodes.Call, getReturnValue);
                            il.Emit(OpCodes.Unbox_Any, methodInfo.ReturnType);
                            il.MarkLabel(afterUnbox);
                        }
                        else
                        {
                            il.Emit(OpCodes.Castclass, methodInfo.ReturnType);
                        }
                    }

                    il.Emit(OpCodes.Ret);
                }
            }

            staticIl.Emit(OpCodes.Ret);

            Type proxyType = type.CreateType();

            assembly.Save("test.dll");

            return proxyType;
        }
    }
}