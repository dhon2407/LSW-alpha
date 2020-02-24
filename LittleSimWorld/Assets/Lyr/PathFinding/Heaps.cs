using System;

namespace PathFinding {

	public class ClosedHeap {

		public Node[] nodes;
		public int currentCount;

		public ClosedHeap(int capacity) { nodes = new Node[capacity]; }
		public void Clear() { for (int i = 0; i < nodes.Length; i++) { nodes[i] = null; } currentCount = 0; }
		public bool Contains(Node item) { return nodes[item.heapIndex] == item; }
		public void Add(Node item) {
			item.heapIndex = currentCount;
			nodes[item.heapIndex] = item;
			currentCount++;
		}
	}


	public class OpenHeap {

		Node[] items;
		public int Count;

		public OpenHeap(int maxHeapSize) { items = new Node[maxHeapSize]; }
		public void Clear() { for (int i = 0; i < items.Length; i++) { items[i] = null; } Count = 0; }
		public bool Contains(Node item) { return items[item.heapIndex] == item; }
		public void Add(Node item) {
			item.heapIndex = Count;
			items[Count] = item;
			Count++;

			//Parent swapping
			int parentIndex = (item.heapIndex - 1) / 2;
			Node parentItem = items[parentIndex];
			
			while (item.IsMoreEfficient(parentItem)) {
				items[item.heapIndex] = parentItem;
				items[parentIndex] = item;
				parentItem.heapIndex = item.heapIndex;
				item.heapIndex = parentIndex;
				parentIndex = (item.heapIndex - 1) / 2;
				parentItem = items[parentIndex];
			}
		}

		public Node RemoveFirst() {
			Node firstItem = items[0];
			Count--;
			Node item = items[0] = items[Count];
			item.heapIndex = 0;

			//Children swapping
			//int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexLeft = ((item.heapIndex << 1) + 1);
			int childIndexRight = childIndexLeft + 1;
			int swapIndex = childIndexLeft;
			while (childIndexLeft < Count) {
				if (childIndexRight < Count && !items[childIndexLeft].IsMoreEfficient(items[childIndexRight])) { swapIndex = childIndexRight; }
				Node swapItem = items[swapIndex];
				if (!item.IsMoreEfficient(swapItem)) {
					items[item.heapIndex] = swapItem;
					items[swapIndex] = item;
					swapItem.heapIndex = item.heapIndex;
					item.heapIndex = swapIndex;
				}
				else { break; }
				childIndexLeft = swapIndex = childIndexRight = ((item.heapIndex << 1) + 1);
				++childIndexRight;
			}
			return firstItem;
		}
	}

	public interface IHeapItem:IComparable<Node> { int HeapIndex { get; set; } }



}

