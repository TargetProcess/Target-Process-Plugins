// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;

namespace hOOt
{

    #region [ internal classes ]

    internal class KeyPointer
    {
        public KeyPointer(bytearr key, int recno, int duppage)
        {
            RecordNum = recno;
            Key = key;
            DuplicatesPage = duppage;
        }

        public KeyPointer(bytearr key, int recno)
        {
            RecordNum = recno;
            Key = key;
            DuplicatesPage = -1;
        }

        public bytearr Key;
        public int RecordNum;
        public int DuplicatesPage = -1;
    }

    internal struct bytearr
    {
        public bytearr(byte[] key)
        {
            val = key;
        }

        public byte[] val;

        public override int GetHashCode()
        {
            int result = 17;
            for (int i = 0; i < val.Length; ++i)
            {
                result = result * 31 + val[i];
            }
            return result;
        }
    }

    #endregion

    internal class Bucket
    {
        internal int BucketNumber = -1;
        internal List<KeyPointer> Pointers;
        internal List<int> Duplicates;
        internal int DiskPageNumber = -1;
        internal int NextPageNumber = -1;
        internal bool isDirty = false;
        internal bool isBucket = true;
        internal bool isOverflow = false;

        public Bucket(byte type, int bucketnumber, List<KeyPointer> pointers, List<int> duplicates, int diskpage, int nextpage)
        {
            DiskPageNumber = diskpage;
            BucketNumber = bucketnumber;
            NextPageNumber = nextpage;
            if ((type & 8) == 8)
                isBucket = true;
            if ((type & 16) == 16)
                isOverflow = true;
            Pointers = pointers;
            Duplicates = duplicates;
        }

        public Bucket(int page)
        {
            DiskPageNumber = page;
            Pointers = new List<KeyPointer>();
            Duplicates = new List<int>();
        }

        public short Count
        {
            get { return (short) Pointers.Count; }
        }
    }
}
