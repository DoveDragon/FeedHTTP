/*
 *
 * Filename: HTTPHeaderFieldCollection.cs
 * 
 * Contains definition of HTTPHeaderFieldCollection class and a double-linked chain algorithm.
 * 
 * Copyright (c) BIT Man Studio 2016-2017. All rights reserved.
 * 
 * MODIFIES LOG:
 * 2016.09.27   MSR     Create file.
 * 2016.09.28   MSR     Completes initial version of the file.
 * 
 * ISSUES LOG:
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace FeedHTTP.Network
{
    public sealed class HTTPHeaderFieldsCollection : ICollection<HTTPHeaderField>
    {
        /// <summary>
        /// Represent a node in a double-linked chain.
        /// </summary>
        /// <typeparam name="T">The type of the elements contained in the node.</typeparam>
        private sealed class DoubleLinkedChainNode<T>
        {
            /// <summary>
            /// Get or set the next node.
            /// </summary>
            public DoubleLinkedChainNode<T> Previous { get; private set; }

            /// <summary>
            /// Get or set the previous node.
            /// </summary>
            public DoubleLinkedChainNode<T> Next { get; private set; }

            /// <summary>
            /// Get the element contained in the current node.
            /// </summary>
            public T Value { get; set; }

            /// <summary>
            /// Construct a DoubleLinkedChainNode with the value contained in the node.
            /// </summary>
            /// <param name="value">The value contained in the node.</param>
            public DoubleLinkedChainNode(T value)
            {
                Value = value;
                Previous = null;
                Next = null;
            }

            /// <summary>
            /// Get if the current node is the first node of a chain.
            /// </summary>
            /// <returns>A bool value indicating if the current node is the first node 
            /// of a chain.</returns>
            public bool IsFirst()
            {
                return Previous == null;
            }

            /// <summary>
            /// Get if the current node is the last node of a chain.
            /// </summary>
            /// <returns>A bool value indicating if the current node is the first node of
            /// a chain.</returns>
            public bool IsLast()
            {
                return Next == null;
            }

            /// <summary>
            /// Connect the two given chain nodes together.
            /// </summary>
            /// <param name="left">The nodes at the left side.</param>
            /// <param name="right">The nodes at the right side.</param>
            /// <exception cref="ArgumentNullException">
            /// Occurs when left or right is null.
            /// </exception>
            public static void ConnectChainNodes(DoubleLinkedChainNode<HTTPHeaderField> left,
                DoubleLinkedChainNode<HTTPHeaderField> right)
            {
                if (left == null)
                    throw new ArgumentNullException(nameof(left));
                if (right == null)
                    throw new ArgumentNullException(nameof(right));

                left.Next = right;
                right.Previous = left;
            }

            /// <summary>
            /// Destroy the connection between the given chain node and its next chain node.
            /// </summary>
            /// <param name="node">The chain node to perform disconnect operation on.</param>
            /// <exception cref="ArgumentNullException">
            /// Occurs when node is null.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Occurs when node.Next is null.
            /// </exception>
            public static void DisconnectChainNodes(DoubleLinkedChainNode<HTTPHeaderField> node)
            {
                if (node == null)
                    throw new ArgumentNullException(nameof(node));
                if (node.Next == null)
                    throw new ArgumentException(nameof(node));

                node.Next.Previous = null;
                node.Next = null;
            }
        }

        /// <summary>
        /// Represent an enumrator to go through all nodes in a double-linked chain.
        /// </summary>
        /// <typeparam name="T">The type of the value contained in each chain node.</typeparam>
        private sealed class DoubleLinkedChainEnumrator<T> : IEnumerator<T>
        {
            private DoubleLinkedChainNode<T> m_first;
            private DoubleLinkedChainNode<T> m_last;
            private DoubleLinkedChainNode<T> m_current;

            /// <summary>
            /// Construct a DoubleLinkedChainEnumrator with the first and the last chain node
            /// in a chain. 
            /// </summary>
            /// <param name="first"></param>
            /// <param name="last"></param>
            /// <remarks>
            /// The newly constructed iterator is in a state called "initial state"
            /// in which the iterator refers to a virtual chain node one before the first chain node.
            /// </remarks>
            public DoubleLinkedChainEnumrator(DoubleLinkedChainNode<T> first,
                DoubleLinkedChainNode<T> last)
            {
                if (m_first != null && m_last == null)
                    throw new ArgumentException("m_first is not null but m_last is null.");

                m_first = first;
                m_last = last;
                m_current = null;
            }

            /// <summary>
            /// Get the current value that the iterator now refers to.
            /// </summary>
            /// <exception cref="InvalidOperationException">
            /// Occurs when the iterator is at initial state.
            /// </exception>
            public T Current
            {
                get
                {
                    if (m_current == null)
                        throw new InvalidOperationException("The iterator is at initial state.");

                    return m_current.Value;
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            /// <summary>
            /// Get if the chain that this iterator refers to is an empty chain.
            /// </summary>
            /// <returns>A bool value indicating if the chain that this iterator refers
            /// to is an empty chain.</returns>
            private bool IsEmpty()
            {
                return m_first == null;
            }

            /// <summary>
            /// Dispose the current iterator. After disposing, the iterator refers to an
            /// empty chain.
            /// </summary>
            public void Dispose()
            {
                m_first = null;
                m_last = null;
            }

            /// <summary>
            /// Move the iterator to the next chain node. If the iterator refers to the last node
            /// currently or the iterator refers to an empty chain, MoveNext will do nothing 
            /// but return false.
            /// </summary>
            /// <returns>A bool value indicating if the iterator had arrived the last node
            /// before moving.</returns>
            public bool MoveNext()
            {
                if (IsEmpty())
                    return false;
                if (m_first == null)
                {
                    // Currently the iterator is in initial state.
                    m_current = m_first;
                    return true;
                }
                else
                {
                    if (ReferenceEquals(m_current, m_last))
                    {
                        // Currently the iterator is refering to the last node in the chain.
                        return false;
                    }
                    else
                    {
                        // Move the iterator to the next chain node.
                        m_current = m_current.Next;
                        return true;
                    }
                }
            }

            /// <summary>
            /// Reset the iterator to initial state.
            /// </summary>
            public void Reset()
            {
                m_current = null;
            }
        }

        // Refers to the first element in the double-linked chain.
        private DoubleLinkedChainNode<HTTPHeaderField> m_first; 
        // Refers to the last element in the double-linked chain.
        private DoubleLinkedChainNode<HTTPHeaderField> m_last;  
        // Refers to the number of chain nodes(elements) contained in the chain(collection).
        private int m_count;

        /// <summary>
        /// Get the number of elements in the collection.
        /// </summary>
        public int Count
        {
            get { return m_count; }
        }

        /// <summary>
        /// Get a bool value indicating if this collection is readonly. Always get false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Default construct an HTTPHeaderFieldsCollection object.
        /// </summary>
        public HTTPHeaderFieldsCollection()
        {
            m_first = null;
            m_last = null;
            m_count = 0;
        }

        /// <summary>
        /// Add a new HTTPHeaderField object to the collection.
        /// </summary>
        /// <param name="item">The HTTPHeaderField object to be added.</param>
        /// <exception cref="ArgumentNullException"/>
        public void Add(HTTPHeaderField item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (m_last == null)
            {
                // There're no elements in the chain.
                m_first = new DoubleLinkedChainNode<HTTPHeaderField>(item);
                m_last = m_first;
            }
            else
            {
                // There're at least one element in the chain.
                // Adds the newly-added element at the end of the chain.
                DoubleLinkedChainNode<HTTPHeaderField>.ConnectChainNodes(m_last,
                    new DoubleLinkedChainNode<HTTPHeaderField>(item));
                // Updates m_last to refer to the last node in the chain, which is the newly-added
                // chain node.
                m_last = m_last.Next;
            }
            // Update element counter.
            ++m_count;
        }

        /// <summary>
        /// Remove all the elements contained in this collection.
        /// </summary>
        public void Clear()
        {
            // We write the while condition as m_count > 1 but not m_count > 0
            // because disconnecting an independent chain node is meaningless.
            while (m_count > 1)
            {
                // Disconnect the last chain node.
                m_last = m_last.Previous;
                DoubleLinkedChainNode<HTTPHeaderField>.DisconnectChainNodes(m_last);
                // Update elements counter.
                --m_count;
            }
            // There're still one elements left in the collection.
            // Set both m_first and m_last to null to indicate that the collection is empty.
            m_first = null;
            m_last = null;
        }

        /// <summary>
        /// Iterate the chain to test if the specified field is in the collection.
        /// </summary>
        /// <param name="item">The field to find in the collection.</param>
        /// <returns>
        /// A value of type bool indicating if the given HTTPHeaderField object
        /// is in the collection.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Occurs when item is null.
        /// </exception>
        /// <remarks>
        /// This method uses RefrenceEquals method to judge if two objects are the same.
        /// </remarks>
        public bool Contains(HTTPHeaderField item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            foreach (HTTPHeaderField field in this)
            {
                if (ReferenceEquals(item, field))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Iterate the chain to test if the specified name of a field is in the collection.
        /// </summary>
        /// <param name="fieldName">The name of a field to find in the collection.</param>
        /// <returns>
        /// A value of type bool indicating if the given name of a field is in
        /// the collection.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Occurs when fieldName is null.
        /// </exception>
        public bool ContainsName(string fieldName)
        {
            if (fieldName == null)
                throw new ArgumentNullException(nameof(fieldName));

            foreach (HTTPHeaderField field in this)
            {
                // The comparison of a field name is case-ignored.
                if (string.Compare(field.Name, fieldName, true) == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Copy the element contained in the collection to the given array.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="arrayIndex">The start index to store the copied elements.</param>
        /// <exception cref="ArgumentNullException">
        /// Occurs when array is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Occurs when array is null.
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// Occurs when arrayIndex is less than 0 or arrayIndex is more than or equals to the
        /// length of array.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Occurs when space in the target array is not big enough to hold the elements in
        /// the collection.
        /// </exception>
        public void CopyTo(HTTPHeaderField[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex >= array.Length)
                throw new IndexOutOfRangeException("arrayIndex is out of range.");
            if (array.Length - arrayIndex < m_count)
                throw new ArgumentException("Target array is too small.");

            IEnumerator<HTTPHeaderField> enumrator = GetEnumerator();
            for (; arrayIndex < array.Length; ++arrayIndex)
            {
                if (!enumrator.MoveNext())
                    throw new Exception();      // Unexpected error occurs.

                array[arrayIndex] = enumrator.Current;
            }
        }

        /// <summary>
        /// Find a HTTPHeaderField in the collection with its name.
        /// </summary>
        /// <param name="fieldName">The name of the HTTPHeaderField to find.</param>
        /// <returns>
        /// If succeefully find an expected object, return that object; returns null otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Occurs when fieldName is null.
        /// </exception>
        /// <remarks>
        /// The name of a field is case-ignoring.
        /// </remarks>
        public HTTPHeaderField Find(string fieldName)
        {
            if (fieldName == null)
                throw new ArgumentNullException(nameof(fieldName));

            foreach (HTTPHeaderField field in this)
            {
                if (string.Compare(field.Name, fieldName, true) == 0)
                    return field;
            }
            return null;
        }

        /// <summary>
        /// Remove the given HTTPHeaderField object from the collection.
        /// </summary>
        /// <param name="item">The object to be removed.</param>
        /// <returns>A value of type bool indicating if the operation succeed.</returns>
        /// <exception cref="ArgumentNullException">
        /// Occurs when item is null.
        /// </exception>
        /// <remarks>
        /// This method uses RefrenceEquals to judge if two objects are the same.
        /// This method is probably time-consuming.
        /// </remarks>
        public bool Remove(HTTPHeaderField item)
        {
            if (item == null)
                throw new ArgumentNullException();
            if (m_first == null)
            {
                // There're no elements in the collection.
                return false;
            }

            // Iterate through the chain to fetch the target node.
            DoubleLinkedChainNode<HTTPHeaderField> current = m_first;
            do
            {
                if (ReferenceEquals(current, item))
                {
                    // Cut the connection between current and its surrounding nodes.
                    DoubleLinkedChainNode<HTTPHeaderField> next = current.Next;
                    DoubleLinkedChainNode<HTTPHeaderField> prev = current.Previous;
                    if (next != null)
                    {
                        // Cut the connection between current and its next node.
                        DoubleLinkedChainNode<HTTPHeaderField>.DisconnectChainNodes(current);
                    }
                    if (prev != null)
                    {
                        // Cut the connection between current and its previous node.
                        DoubleLinkedChainNode<HTTPHeaderField>.DisconnectChainNodes(prev);
                    }
                    // Update elements counter.
                    --m_count;

                    // Update the referers in prev and next.
                    if (prev == null && next == null)
                    {
                        // There was only one element before removing.
                        // Now there should be no elements in the collection.
                        m_first = null;
                        m_last = null;
                    }
                    else if (prev == null)
                    {
                        // prev is null but next is not null.
                        // This case occurs when the removed elements was the first element.
                        m_first = next;
                    }
                    else if (next == null)
                    {
                        // next is null but prev is not null.
                        // This case occurs when the removed elements was the last element.
                        m_last = prev;
                    }
                    else
                    {
                        // Either next nor prev is null.
                        // Connect the two nodes represented by prev and next.
                        DoubleLinkedChainNode<HTTPHeaderField>.ConnectChainNodes(prev, next);
                    }

                    return true;
                }
                current = current.Next;
            } while (current != null);
            return false;
        }

        /// <summary>
        /// Get the enumrator of the collection.
        /// </summary>
        /// <returns>The enumrator of the collection.</returns>
        public IEnumerator<HTTPHeaderField> GetEnumerator()
        {
            return new DoubleLinkedChainEnumrator<HTTPHeaderField>(m_first, m_last);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
