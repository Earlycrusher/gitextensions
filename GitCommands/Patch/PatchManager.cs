            ChunkList selectedChunks = ChunkList.GetSelectedChunks(text, selectionPosition, selectionLength, out var header);
        public static byte[] GetSelectedLinesAsPatch(string text, int selectionPosition, int selectionLength, bool staged, Encoding fileContentEncoding, bool isNewFile)
            ChunkList selectedChunks = ChunkList.GetSelectedChunks(text, selectionPosition, selectionLength, out var header);
            string[] headerLines = header.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var s = new StringBuilder();
            byte[] bs = Encoding.UTF8.GetBytes(input);
        public string WasNoNewLineAtTheEnd;
        public string IsNoNewLineAtTheEnd;
        private SubChunk _currentSubChunk;
            string[] lines = fileText.Split(new[] { eol }, StringSplitOptions.None);
        public static ChunkList GetSelectedChunks(string text, int selectionPosition, int selectionLength, out string header)
            string[] chunks = diff.Split(new[] { "\n@@" }, StringSplitOptions.RemoveEmptyEntries);