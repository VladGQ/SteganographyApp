using System.Linq;
using System.Collections.Generic;

using SteganographyApp.Common.IO;

namespace SteganographyApp
{

    /// <summary>
    /// Hooks into the image store's chunk written event to
    /// record the length of a binary chunk once it has been written to a
    /// storage image so the entire content chunk table can be written
    /// to the leading image at the end of the encoding process.
    /// </summary>
    public class TableChunkTracker
    {

        /// <summary>
        /// Contains the ordered list of chunks that have been written to the storage images.
        /// </summary>
        private readonly LinkedList<int> contentChunks = new LinkedList<int>();

        private LinkedListNode<int> lastNode = null;

        /// <summary>
        /// Returns the current list of content chunks as an array. The original list is a
        /// LinkedList meaning the order of this array matches the order in which each
        /// content chunk was written to the storage image.
        /// </summary>
        public int[] GetContentTable()
        {
            return contentChunks.ToArray();
        }

        public TableChunkTracker(ImageStore store)
        {
            store.OnChunkWritten += ChunkWritten;
        }

        private void ChunkWritten(object sender, ChunkWrittenArgs args)
        {
            LinkedListNode<int> node = new LinkedListNode<int>(args.ChunkLength);
            if (lastNode == null)
            {
                contentChunks.AddFirst(node);
            }
            else
            {
                contentChunks.AddAfter(lastNode, node);
            }
            lastNode = node;
        }

    }

}