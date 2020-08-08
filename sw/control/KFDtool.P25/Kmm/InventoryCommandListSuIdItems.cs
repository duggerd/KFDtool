﻿using System;

namespace KFDtool.P25.Kmm
{
    public class InventoryCommandListSuIdItems : KmmBody
    {
        public int InventoryMarker { get; private set; }

        public int MaxSuIdRequested { get; private set; }

        public override MessageId MessageId
        {
            get
            {
                return MessageId.InventoryCommand;
            }
        }

        public InventoryType InventoryType
        {
            get
            {
                return InventoryType.ListSuIdItems;
            }
        }

        public override ResponseKind ResponseKind
        {
            get
            {
                return ResponseKind.Immediate;
            }
        }

        public InventoryCommandListSuIdItems(int inventoryMarker, int maxSuIdRequested)
        {
            if (inventoryMarker < 0 || inventoryMarker > 0xFFFFFF)
            {
                throw new ArgumentOutOfRangeException("inventoryMarker");
            }

            if (maxSuIdRequested < 0 || maxSuIdRequested > 0xFFFF)
            {
                throw new ArgumentOutOfRangeException("maxSuIdRequested");
            }

            InventoryMarker = inventoryMarker;
            MaxSuIdRequested = maxSuIdRequested;
        }

        public override byte[] ToBytes()
        {
            byte[] contents = new byte[6];

            /* inventory type */
            contents[0] = (byte)InventoryType;

            /* inventory marker */
            contents[1] = (byte)((InventoryMarker >> 16) & 0xFF);
            contents[2] = (byte)((InventoryMarker >> 8) & 0xFF);
            contents[3] = (byte)(InventoryMarker & 0xFF);

            /* max number of suid requested */
            contents[4] = (byte)((MaxSuIdRequested >> 8) & 0xFF);
            contents[5] = (byte)(MaxSuIdRequested & 0xFF);

            return contents;
        }

        public override void Parse(byte[] contents)
        {
            throw new NotImplementedException();
        }
    }
}
