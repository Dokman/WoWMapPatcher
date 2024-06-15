using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

class Patcher
{
    const int OccLocOffset = 0x6EDEC0;
    const int OccTable1Offset = 0x6EE038;
    const int OccTable1Size = 62;
    const int OccLocSize = 32;

    [StructLayout(LayoutKind.Sequential)]
    struct OccTableEntry
    {
        public uint unk1;
        public uint unk2;
        public uint mapid;
        public uint unk3;
        public uint unk4;
        public uint unk5;
        public uint unk6;
        public uint unk7;
        public uint unk8;
        public uint unk9;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct OccLocEntry
    {
        public float Loc0;
        public float Loc1;
        public float Loc2;
    }

    static void Main(string[] args)
    {
        string filePath = "Wow.exe";
        string logFilePath = "occluder_log.txt";

        OccTableEntry[] occTable = new OccTableEntry[OccTable1Size];
        OccLocEntry[] occLoc = new OccLocEntry[OccLocSize];

        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            using (BinaryReader reader = new BinaryReader(fs))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                // Read Occluder Table
                fs.Seek(OccTable1Offset, SeekOrigin.Begin);
                for (int i = 0; i < OccTable1Size; i++)
                {
                    occTable[i] = new OccTableEntry
                    {
                        unk1 = reader.ReadUInt32(),
                        unk2 = reader.ReadUInt32(),
                        mapid = reader.ReadUInt32(),
                        unk3 = reader.ReadUInt32(),
                        unk4 = reader.ReadUInt32(),
                        unk5 = reader.ReadUInt32(),
                        unk6 = reader.ReadUInt32(),
                        unk7 = reader.ReadUInt32(),
                        unk8 = reader.ReadUInt32(),
                        unk9 = reader.ReadUInt32()
                    };
                }


                /// Disable this if you want to only log the Entrys
                // Modify Entrys with mapId 9999
                fs.Seek(OccTable1Offset, SeekOrigin.Begin);
                for (int i = 0; i < OccTable1Size; i++)
                {
                    if (occTable[i].mapid != 9999)
                    {
                        occTable[i].mapid = 9999; 

                        writer.Write(occTable[i].unk1);
                        writer.Write(occTable[i].unk2);
                        writer.Write(occTable[i].mapid);
                        writer.Write(occTable[i].unk3);
                        writer.Write(occTable[i].unk4);
                        writer.Write(occTable[i].unk5);
                        writer.Write(occTable[i].unk6);
                        writer.Write(occTable[i].unk7);
                        writer.Write(occTable[i].unk8);
                        writer.Write(occTable[i].unk9);
                    }
                    else
                    {
                        // skip
                        fs.Seek(40, SeekOrigin.Current); // Jump 10 uints (40 bytes)
                    }
                }

                // Read Occluder Locations
                fs.Seek(OccLocOffset, SeekOrigin.Begin);
                for (int i = 0; i < OccLocSize; i++)
                {
                    occLoc[i] = new OccLocEntry
                    {
                        Loc0 = reader.ReadSingle(),
                        Loc1 = reader.ReadSingle(),
                        Loc2 = reader.ReadSingle()
                    };
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
            return;
        }

        // Write log
        using (StreamWriter log = new StreamWriter(logFilePath))
        {
            log.WriteLine("Occluder Table:");
            for (int i = 0; i < OccTable1Size; i++)
            {
                log.WriteLine($"Entry {i + 1}: mapid = {occTable[i].mapid}");
            }

            log.WriteLine("\nOccluder Locations:");
            for (int i = 0; i < OccLocSize; i++)
            {
                log.WriteLine($"Entry {i + 1}: Loc0 = {occLoc[i].Loc0}, Loc1 = {occLoc[i].Loc1}, Loc2 = {occLoc[i].Loc2}");
            }
        }

        Console.WriteLine($"Log created at {logFilePath}");
    }
}
