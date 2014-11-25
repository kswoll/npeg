using System;

namespace PEG.Utils
{
    public class BooleanSet
    {
        private ulong[] partitions;
        //        private DynamicArray<ulong> partitions = new DynamicArray<ulong>();
        private int count;
        private const int partitionSize = 64;
        private const int segmentSize = 8;

        private BooleanSet()
        {
        }

        public BooleanSet(int size)
        {
            int length = size / partitionSize;
            if (size % partitionSize > 0)
                length++;
            partitions = new ulong[length];
        }

        public bool this[int data]
        {
            get { return Contains(data); }
            set
            {
                if (value)
                    Add(data);
                else
                    Remove(data);
            }
        }

        public void Add(int value)
        {
            int partitionIndex = value / partitionSize;
            int indexInPartition = value % partitionSize;
            int segmentIndex = indexInPartition / segmentSize;
            int bitIndex = indexInPartition % segmentSize;

            ulong partition = partitions[partitionIndex];
            byte segment = (byte)((partition >> (segmentIndex * segmentSize)) & 0xFF);

            byte mask = (byte)((long)Math.Pow(2, bitIndex));
            bool exists = (segment & mask) != 0;

            if (!exists)
            {
                segment |= mask;
                partition |= (ulong)segment << (segmentIndex * segmentSize);
                partitions[partitionIndex] = partition;
                count++;
            }
        }

        public void Remove(int value)
        {
            int partitionIndex = value / partitionSize;
            int indexInPartition = value % partitionSize;
            int segmentIndex = indexInPartition / segmentSize;
            int bitIndex = indexInPartition % segmentSize;

            ulong partition = partitions[partitionIndex];
            byte segment = (byte)((partition >> (segmentIndex * 8)) & 0xFF);

            byte mask = (byte)((long)Math.Pow(2, bitIndex));
            bool exists = (segment & mask) != 0;

            if (exists)
            {
                segment &= (byte)~mask;
                partition = (BitShift.ByteMasks[segmentIndex] & partition) | ((ulong)segment << (segmentIndex * segmentSize));
                partitions[partitionIndex] = partition;
                count--;
            }
        }

        public bool Contains(int value)
        {
            int partitionIndex = value / partitionSize;
            int indexInPartition = value % partitionSize;
            int segmentIndex = indexInPartition / segmentSize;
            int bitIndex = indexInPartition % segmentSize;

            ulong partition = partitions[partitionIndex];
            byte segment = (byte)((partition >> (segmentIndex * 8)) & 0xFF);

            byte mask = (byte)((long)Math.Pow(2, bitIndex));
            bool exists = (segment & mask) != 0;
            return exists;
        }

        public BooleanSet Copy()
        {
            BooleanSet copy = new BooleanSet();
            copy.partitions = new ulong[partitions.Length];
            partitions.CopyTo(copy.partitions, 0);
            return copy;
        }
    }
}