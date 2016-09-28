/*
 *
 * Filename: HTTPHeaderField.cs
 * 
 * Contains definition of HTTPHeaderField class.
 * 
 * Copyright (c) BIT Man Studio 2016-2017. All rights reserved.
 * 
 * MODIFIES LOG:
 * 2016.09.27   MSR     Create file.
 * 2016.09.27   MSR     Completes initial version of the file.
 * 
 * ISSUES LOG:
 * 
 */

namespace FeedHTTP.Network
{
    /// <summary>
    /// Represent a field in a HTTP message's header.
    /// </summary>
    public sealed class HttpHeaderField
    {
        /// <summary>
        /// Get the name of the field.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Get the value of the field.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Constructs an HTTPHeaderField object with the given name and value.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="value">The value of the field.</param>
        public HttpHeaderField(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Compares current object with the specified object and returns a bool indicating
        /// if the two objects are the same.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            HttpHeaderField objField = obj as HttpHeaderField;
            if (objField == null)
                return false;

            // The two objects are the same if their names are the same ignoring case and their values
            // are the same.
            return string.Compare(Name, objField.Name, true) == 0
                && string.Compare(Value, objField.Value, false) == 0;
        }

        /// <summary>
        /// Get hash code of current object.
        /// </summary>
        /// <returns>The hash code of current object.</returns>
        public override int GetHashCode()
        {
            // We make name to lower to ignoring the case of name.
            return unchecked(Name.ToLower().GetHashCode() + Value.GetHashCode());
        }

        /// <summary>
        /// Compare the two specified HTTPHeaderField objects and return a bool indicating
        /// if the two objects are the same.
        /// </summary>
        /// <param name="f1">The left-hand operand of == operator.</param>
        /// <param name="f2">The right-hand operand of == operator.</param>
        /// <returns>A value of type bool indicating if the two objects are the same.</returns>
        public static bool operator ==(HttpHeaderField f1, HttpHeaderField f2)
        {
            if (f1 == null)
            {
                if (f2 == null)
                    return true;
                else
                    return f2.Equals(f1);       // f2 is not null so that we can call 
                                                // f2.Equals(...) safely.
            }
            else
                return f1.Equals(f2);
        }

        /// <summary>
        /// Compare the two specified HTTPHeaderField objects and return a bool indicating
        /// if the two objects are not the same.
        /// </summary>
        /// <param name="f1">The left-hand operand of != operator.</param>
        /// <param name="f2">The right-hand operand of != operator.</param>
        /// <returns>A value of type bool indicating if the two objects are
        /// not the same.</returns>
        public static bool operator !=(HttpHeaderField f1, HttpHeaderField f2)
        {
            return !(f1 == f2);
        }
    }
}
