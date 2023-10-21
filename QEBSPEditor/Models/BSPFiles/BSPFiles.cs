namespace QEBSPEditor.Models.BSPFiles
{
    public static class BSPFiles
    {
        public static IBSPFile Load(Stream stream)
        {
            using var reader = new BinaryReader(stream);

            // First try good ol bsp
            var intVersion = reader.ReadInt32();
            stream.Position = 0;
            if (intVersion == 29)
                return new BSPFile29().Load(stream);

            // Try other bsp versions
            var stringVersion = new string(reader.ReadChars(4));
            stream.Position = 0;
            if (stringVersion == "BSP2")
                return new BSPFileBSP2().Load(stream);
            else if (stringVersion == "2PSB")
                return new BSPFile2PSB().Load(stream);

            // Try generic
            try
            {
                return new BSPFileGeneric().Load(stream);   
            }
            catch(InvalidDataException ex)
            {
                throw new InvalidDataException($"Unsupported BSP version: {stringVersion} | {intVersion}", ex);
            }
            
        }
    }
}
