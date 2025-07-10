// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System.Collections.Generic;

namespace UnitTests.PlayModeTests.Utils
{
	public static class TestUtils
	{
		public static List<T> DefaultList<T>(int count)
		{
			var list = new List<T>();
			for (var i = 0; i < count; i++)
			{
				list.Add(default(T));
			}

			return list;
		}
	}
}