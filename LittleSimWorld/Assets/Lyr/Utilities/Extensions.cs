using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions 
{
	/// <summary> Initialize dictionary of enum as map with either default or new values.
	/// <para>Usage: `public <see cref="Dictionary{TKey, TValue}"/> myDictionary = myDictionary.<see cref="InitializeDefaultValues">InitializeDefaultValues</see>(true);`</para>
	/// </summary>
	/// <typeparam name="T">Type of the <see cref="System.Enum"/> which will be used as a keymap.</typeparam>
	/// <typeparam name="U">Type of the value that will be mapped to the enum.</typeparam>
	/// <param name="dictionary">Dictionary for which the mapping will occur.</param>
	/// <param name="InitializeNew">Should we add new <typeparamref name="T"/>?</param>
	/// <returns>A dictionary with <typeparamref name="T"/> as a keymap.</returns>
	public static Dictionary<T,U> InitializeDefaultValues<T, U>(this Dictionary<T, U> dictionary, bool InitializeNew) where T : System.Enum where U : new() {

		var enumValues = System.Enum.GetValues(typeof(T));

		if (dictionary == null) {
			//Debug.Log("Dictionary is null. Make sure you assign the value somewhere.");
			dictionary = new Dictionary<T, U>();
		}
		foreach (var enumValue in enumValues) {
			if (InitializeNew) { dictionary.Add((T) enumValue, new U()); }
			else { dictionary.Add((T) enumValue, default(U)); }
		}

		return dictionary;
	}

	/// <summary> Initialize dictionary of enum as map with default values.
	/// <para>Usage: `public <see cref="Dictionary{TKey, TValue}"/> myDictionary = myDictionary.<see cref="InitializeDefaultValues">InitializeDefaultValues</see>();`</para>
	/// </summary>
	/// <typeparam name="T">Type of the <see cref="System.Enum"/> which will be used as a keymap.</typeparam>
	/// <typeparam name="U">Type of the value that will be mapped to the enum.</typeparam>
	/// <param name="dictionary">Dictionary for which the mapping will occur.</param>
	/// <returns>A dictionary with <typeparamref name="T"/> as a keymap.</returns>
	public static Dictionary<T, U> InitializeDefaultValues<T, U>(this Dictionary<T, U> dictionary) where T : System.Enum {

		var enumValues = System.Enum.GetValues(typeof(T));

		if (dictionary == null) { dictionary = new Dictionary<T, U>(); }
		foreach (var enumValue in enumValues) { dictionary.Add((T) enumValue, default(U)); }

		return dictionary;
	}

	/// <summary> Initialize dictionary of enum as map with fixed default values.
	/// <para>Usage: `public <see cref="Dictionary{TKey, TValue}"/> myDictionary = myDictionary.<see cref="InitializeDefaultValues">InitializeDefaultValues</see>(new <typeparamref name="U"/>());`</para>
	/// </summary>
	/// <typeparam name="T">Type of the <see cref="System.Enum"/> which will be used as a keymap.</typeparam>
	/// <typeparam name="U">Type of the value that will be mapped to the enum.</typeparam>
	/// <param name="dictionary">Dictionary for which the mapping will occur.</param>
	/// <param name="defaultValue">The value to initialize the dictionary with. Every enum will be mapped to this value.</param>
	/// <returns>A dictionary with <typeparamref name="T"/> as a keymap.</returns>
	public static Dictionary<T, U> InitializeDefaultValues<T, U>(this Dictionary<T, U> dictionary, U defaultValue) where T : System.Enum {

		var enumValues = System.Enum.GetValues(typeof(T));

		if (dictionary == null) { dictionary = new Dictionary<T, U>(); }
		foreach (var enumValue in enumValues) { dictionary.Add((T) enumValue, defaultValue); }

		return dictionary;
	}
}
