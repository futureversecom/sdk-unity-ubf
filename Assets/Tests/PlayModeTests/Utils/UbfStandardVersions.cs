// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

using System;
using System.Collections;
using Futureverse.UBF.Runtime;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace UnitTests.PlayModeTests.Utils
{	
	[AttributeUsage(AttributeTargets.Parameter)]
	public class UbfStandardVersions : DataAttribute, IParameterDataSource
	{
		private readonly Version _minVersion;
		private readonly Version _maxVersion;
		
		public UbfStandardVersions(string aboveAndIncluding = null, string belowAndIncluding = null)
		{
			if (aboveAndIncluding == null || !Version.TryParse(aboveAndIncluding, out _minVersion))
			{
				_minVersion = VersionUtils.MinSupportedStandardVersion;
			}

			if (belowAndIncluding == null || !Version.TryParse(belowAndIncluding, out _maxVersion))
			{
				_maxVersion = VersionUtils.MaxSupportedStandardVersion;
			}
		}

		public IEnumerable GetData(IParameterInfo parameter)
		{
			if (parameter.ParameterType != typeof(string))
			{
				throw new Exception("UbfStandardVersions: parameter must be a string");
			}
			
			return VersionUtils.EnumerateMinorVersions(_minVersion, _maxVersion);
		}
	}
}