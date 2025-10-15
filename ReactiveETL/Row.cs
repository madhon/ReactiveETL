namespace ReactiveETL;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

/// <summary>
/// Represent a virtual row
/// </summary>
[DebuggerDisplay("Count = {items.Count}")]
[DebuggerTypeProxy(typeof(QuackingDictionaryDebugView))]
[Serializable]
public class Row : QuackingDictionary, IEquatable<Row>
{
    static readonly Dictionary<Type, List<PropertyInfo>> propertiesCache = new Dictionary<Type, List<PropertyInfo>>();
    static readonly Dictionary<Type, List<FieldInfo>> fieldsCache = new Dictionary<Type, List<FieldInfo>>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Row"/> class.
    /// </summary>
    public Row()
        : base(new Hashtable(StringComparer.InvariantCultureIgnoreCase))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Row"/> class.
    /// </summary>
    /// <param name="itemsToClone">The items to clone.</param>
    protected Row(IDictionary itemsToClone)
        : base(new Hashtable(itemsToClone, StringComparer.InvariantCultureIgnoreCase))
    {
    }

    /// <summary>
    /// Creates a copy of the given source, erasing whatever is in the row currently.
    /// </summary>
    /// <param name="source">The source row.</param>
    public void Copy(IDictionary source)
    {
        items = new Hashtable(source, StringComparer.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Gets the columns in this row.
    /// </summary>
    /// <value>The columns.</value>
    public IEnumerable<string> Columns
    {
        get
        {
            //We likely would want to change the row when iterating on the columns, so we
            //want to make sure that we send a copy, to avoid enumeration modified exception
            foreach (string column in new ArrayList(items.Keys))
            {
                yield return column;
            }
        }
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns></returns>
    public Row Clone()
    {
        var row = new Row(this);
        return row;
    }

    /// <summary>
    /// Indicates whether the current <see cref="Row" /> is equal to another <see cref="Row" />.
    /// </summary>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public bool Equals(Row other)
    {
        if(Columns.SequenceEqual(other.Columns, StringComparer.InvariantCultureIgnoreCase) == false)
            return false;

        foreach (var key in items.Keys)
        {
            var item = items[key];
            var otherItem = other.items[key];

            if (item == null | otherItem == null)
                return item == null & otherItem == null;

            var equalityComparer = CreateComparer(item.GetType(), otherItem.GetType());

            if(equalityComparer(item, otherItem) == false)
                return false;
        }

        return true;
    }

    private static Func<object, object, bool> CreateComparer(Type firstType, Type secondType)
    {
        if (firstType == secondType)
            return Equals;

        var firstParameter = Expression.Parameter(typeof (object), "first");
        var secondParameter = Expression.Parameter(typeof (object), "second");

        var equalExpression = Expression.Equal(Expression.Convert(firstParameter, firstType), 
            Expression.Convert(Expression.Convert(secondParameter, secondType), firstType));

        return Expression.Lambda<Func<object, object, bool>>(equalExpression, firstParameter, secondParameter).Compile();
    }

    /// <summary>
    /// Creates a key from the current row, suitable for use in hashtables
    /// </summary>
    public ObjectArrayKeys CreateKey()
    {
        return CreateKey(null);
    }

    /// <summary>
    /// Creates a key that allow to do full or partial indexing on a row
    /// </summary>
    /// <param name="columns">The columns.</param>
    /// <returns></returns>
    public ObjectArrayKeys CreateKey(params string[] columns)
    {
        object[] array = new object[columns.Length];
        for (int i = 0; i < columns.Length; i++)
        {
            array[i] = items[columns[i]];
        }
        return new ObjectArrayKeys(array);
    }

    /// <summary>
    /// Copy all the public properties and fields of an object to the row
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns></returns>
    public static Row FromObject(object obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var row = new Row();
        foreach (PropertyInfo property in GetProperties(obj))
        {
            row[property.Name] = property.GetValue(obj, new object[0]);
        }
        foreach (FieldInfo field in GetFields(obj))
        {
            row[field.Name] = field.GetValue(obj);
        }
        return row;
    }

    private static List<PropertyInfo> GetProperties(object obj)
    {
        if (propertiesCache.TryGetValue(obj.GetType(), out var properties))
            return properties;

        properties = new List<PropertyInfo>();
        foreach (PropertyInfo property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
        {
            if (property.CanRead == false || property.GetIndexParameters().Length > 0)
                continue;
            properties.Add(property);
        }
        propertiesCache[obj.GetType()] = properties;
        return properties;
    }

    private static List<FieldInfo> GetFields(object obj)
    {
        if (fieldsCache.TryGetValue(obj.GetType(), out var fields))
            return fields;

        fields = new List<FieldInfo>();
        foreach (FieldInfo fieldInfo in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
        {
            fields.Add(fieldInfo);
        }
        fieldsCache[obj.GetType()] = fields;
        return fields;
    }

    /// <summary>
    /// Generate a row from the reader
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    public static Row FromReader(IDataReader reader)
    {
        var row = new Row();
        row.Add(reader);
        return row;
    }

    /// <summary>
    /// Add datareader to a row
    /// </summary>
    /// <param name="reader"></param>
    public void Add(IDataReader reader)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            this[reader.GetName(i)] = reader.GetValue(i);
        }
    }

    /// <summary>
    /// Create a new object of <typeparamref name="T"/> and set all
    /// the matching fields/properties on it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T ToObject<T>()
    {
        return (T)ToObject(typeof(T));
    }

    /// <summary>
    /// Create a new object of <param name="type"/> and set all
    /// the matching fields/properties on it.
    /// </summary>
    public object ToObject(Type type)
    {
        object instance = Activator.CreateInstance(type);
        foreach (PropertyInfo info in GetProperties(instance))
        {
            if(items.Contains(info.Name) && info.CanWrite)
                info.SetValue(instance, items[info.Name],null);
        }
        foreach (FieldInfo info in GetFields(instance))
        {
            if(items.Contains(info.Name))
                info.SetValue(instance,items[info.Name]);
        }
        return instance;
    }

    /// <summary>
    /// Set a value in the row
    /// </summary>
    /// <param name="key">key of the value</param>
    /// <param name="val">value to put in the row</param>
    /// <returns>current row</returns>
    public Row Set(string key, object val)
    {
        this[key] = val;
        return this;
    }
}