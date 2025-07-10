// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using Futureverse.UBF.Runtime;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

public class TestDynamics
{
	[Test]
	public void DynamicString()
	{
		const string sampleString = "Foo";
		var dynamicString = Dynamic.String(sampleString);

		// A Dynamic string should be able to be dereferenced into the original string
		Assert.IsTrue(dynamicString.TryReadString(out var dereferencedString));
		Assert.AreEqual(dereferencedString, sampleString);

		// Should not be able to push to a non-array
		Assert.IsFalse(dynamicString.Push(Dynamic.String("Bar")));

		// Dynamic string should not be interpreted as a different Dynamic type
		Assert.IsFalse(dynamicString.TryReadInt(out _));
		Assert.IsFalse(dynamicString.TryReadFloat(out _));
		Assert.IsFalse(dynamicString.TryReadBoolean(out _));
		Assert.IsFalse(dynamicString.TryInterpretAs<object>(out _));
	}
	
	[Test]
	public void DynamicFloat()
	{
		const float sampleFloat = 2.1f;
		var dynamicFloat = Dynamic.Float(sampleFloat);

		// A Dynamic float should be able to be dereferenced into the original float
		Assert.IsTrue(dynamicFloat.TryReadFloat(out var dereferencedFloat));
		Assert.AreEqual(sampleFloat, dereferencedFloat, float.Epsilon);

		// Dynamic float should not be interpreted as a different Dynamic type
		Assert.IsFalse(dynamicFloat.TryReadInt(out _));
		Assert.IsFalse(dynamicFloat.TryReadString(out _));
		Assert.IsFalse(dynamicFloat.TryReadBoolean(out _));
		Assert.IsFalse(dynamicFloat.TryInterpretAs<object>(out _));
	}

	[Test]
	public void DynamicInt()
	{
		const int sampleInt = 143;
		var dynamicInt = Dynamic.Int(sampleInt);

		// A Dynamic float should be able to be dereferenced into the original float
		Assert.IsTrue(dynamicInt.TryReadInt(out var dereferencedInt));
		Assert.AreEqual(sampleInt, dereferencedInt);

		// Dynamic float should not be interpreted as a different Dynamic type
		Assert.IsFalse(dynamicInt.TryReadFloat(out _));
		Assert.IsFalse(dynamicInt.TryReadString(out _));
		Assert.IsFalse(dynamicInt.TryReadBoolean(out _));
		Assert.IsFalse(dynamicInt.TryInterpretAs<object>(out _));
	}

	[Test]
	public void DynamicBool()
	{
		const bool sampleBool = true;
		var dynamicBool = Dynamic.Bool(sampleBool);

		// A Dynamic float should be able to be dereferenced into the original float
		Assert.IsTrue(dynamicBool.TryReadBoolean(out var dereferencedBool));
		Assert.AreEqual(sampleBool, dereferencedBool);

		// Dynamic float should not be interpreted as a different Dynamic type
		Assert.IsFalse(dynamicBool.TryReadFloat(out _));
		Assert.IsFalse(dynamicBool.TryReadString(out _));
		Assert.IsFalse(dynamicBool.TryReadInt(out _));
		Assert.IsFalse(dynamicBool.TryInterpretAs<object>(out _));
	}

	[Test]
	public void DynamicArray()
	{
		var dynamicArray = Dynamic.Array();

		// Dynamic array should not be interpreted as a different Dynamic type
		Assert.IsFalse(dynamicArray.TryReadFloat(out _));
		Assert.IsFalse(dynamicArray.TryReadString(out _));
		Assert.IsFalse(dynamicArray.TryReadInt(out _));
		Assert.IsFalse(dynamicArray.TryReadBoolean(out _));
		Assert.IsFalse(dynamicArray.TryInterpretAs<object>(out _));

		// Can push a dynamic value to the array
		Assert.IsTrue(dynamicArray.Push(Dynamic.String("Foo")));
		Assert.IsTrue(dynamicArray.Push(Dynamic.String("Bar")));

		var len = 0;

		// Can iterate over array
		dynamicArray.ForEach(
			dynamic =>
			{
				Assert.IsTrue(dynamic.TryReadString(out _));
				len++;
			}
		);

		Assert.AreEqual(len, 2);

		// Can push a value of a different Dynamic type
		Assert.DoesNotThrow(() => dynamicArray.Push(Dynamic.Int(42)));
	}

	[Test]
	public void DynamicDictionary()
	{
		var dynamicDictionary = Dynamic.Dictionary();

		// Dynamic array should not be interpreted as a different Dynamic type
		Assert.IsFalse(dynamicDictionary.TryReadFloat(out _));
		Assert.IsFalse(dynamicDictionary.TryReadString(out _));
		Assert.IsFalse(dynamicDictionary.TryReadInt(out _));
		Assert.IsFalse(dynamicDictionary.TryReadBoolean(out _));
		Assert.IsFalse(dynamicDictionary.TryInterpretAs<object>(out _));

		const string sampleString = "Hello";
		Assert.IsTrue(dynamicDictionary.TrySet("Foo", Dynamic.Float(2.0f)));
		Assert.IsTrue(dynamicDictionary.TrySet("Bar", Dynamic.String(sampleString)));

		Assert.IsTrue(dynamicDictionary.TryGet("Bar", out var dynamicItem));
		Assert.IsTrue(dynamicItem.TryReadString(out var itemString));
		Assert.AreEqual(itemString, sampleString);

		Assert.IsFalse(dynamicDictionary.TryGet("Wrong", out _));
		Assert.IsFalse(dynamicDictionary.TryGet("Wrong", out _));

	}
}