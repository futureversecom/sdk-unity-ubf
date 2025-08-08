// Copyright (c) 2025, Futureverse Corporation Limited. All rights reserved.

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Futureverse.UBF.Runtime.Settings
{
	public class UBFSettings : ScriptableObject
	{
		private const string MyCustomSettingsPath = "Assets/Resources/UBF.asset";
		
		[SerializeField] private AssetReferenceT<Material> _decalOpaque;
		[SerializeField] private AssetReferenceT<Material> _decalTransparent;
		[SerializeField] private AssetReferenceT<Material> _furOpaque;
		[SerializeField] private AssetReferenceT<Material> _furTransparent;
		[SerializeField] private AssetReferenceT<Material> _pbrOpaque;
		[SerializeField] private AssetReferenceT<Material> _pbrTransparent;
		[SerializeField] private AssetReferenceT<Material> _hair;
		[SerializeField] private AssetReferenceT<Material> _skin;
		[SerializeField] private AssetReferenceT<Material> _skin02;
		[SerializeField] private List<MeshConfigEntry> _meshConfigs;
		[SerializeField] private AnimationCurve _lodFalloffCurve;
		public AssetReferenceT<Material> DecalOpaque => _decalOpaque;
		public AssetReferenceT<Material> DecalTransparent => _decalTransparent;
		public AssetReferenceT<Material> FurOpaque => _furOpaque;
		public AssetReferenceT<Material> FurTransparent => _furTransparent;
		public AssetReferenceT<Material> PbrOpaque => _pbrOpaque;
		public AssetReferenceT<Material> PbrTransparent => _pbrTransparent;
		public AssetReferenceT<Material> Hair => _hair;
		public AssetReferenceT<Material> Skin => _skin;
		public AssetReferenceT<Material> Skin02 => _skin02;
		public AnimationCurve LodFalloffCurve => _lodFalloffCurve;

		[Serializable]
		public class MeshConfigEntry
		{
			public string Key;
			public MeshConfig Config;
		}

		public List<MeshConfigEntry> MeshConfigs => _meshConfigs;
		
		public static UBFSettings GetOrCreateSettings()
		{
#if UNITY_EDITOR
			var settings = AssetDatabase.LoadAssetAtPath<UBFSettings>(MyCustomSettingsPath);
			if (settings != null)
			{
				return settings;
			}

			var fullPath = $"{Application.dataPath}/Resources";
			if (!Directory.Exists(fullPath))
			{
				Directory.CreateDirectory(fullPath);
			}

			settings = CreateInstance<UBFSettings>();
			AssetDatabase.CreateAsset(settings, MyCustomSettingsPath);
			AssetDatabase.SaveAssets();
			return settings;
#else
			return UnityEngine.Resources.Load<UBFSettings>("UBF");
#endif
		}

		private void OnEnable()
		{
			OnValidate();
		}

		private void OnValidate()
		{
#if UNITY_EDITOR
			if (_decalOpaque.editorAsset == null)
			{
				var materialPath = FindMaterialPath("M_Decal_Opaque");
				if (materialPath != null)
				{
					var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
					_decalOpaque.SetEditorAsset(material);
				}
			}
			if (_decalTransparent.editorAsset == null)
			{
				var materialPath = FindMaterialPath("M_Decal_Transparent_Alpha");
				if (materialPath != null)
				{
					var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
					_decalTransparent.SetEditorAsset(material);
				}
			}
			if (_furOpaque.editorAsset == null)
			{
				var materialPath = FindMaterialPath("M_Fur_Opaque");
				if (materialPath != null)
				{
					var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
					_furOpaque.SetEditorAsset(material);
				}
			}
			if (_furTransparent.editorAsset == null)
			{
				var materialPath = FindMaterialPath("M_Fur_Transparent_Alpha");
				if (materialPath != null)
				{
					var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
					_furTransparent.SetEditorAsset(material);
				}
			}
			if (_pbrOpaque.editorAsset == null)
			{
				var materialPath = FindMaterialPath("M_PBR_Opaque");
				if (materialPath != null)
				{
					var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
					_pbrOpaque.SetEditorAsset(material);
				}
			}
			if (_pbrTransparent.editorAsset == null)
			{
				var materialPath = FindMaterialPath("M_PBR_Transparent_Alpha");
				if (materialPath != null)
				{
					var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
					_pbrTransparent.SetEditorAsset(material);
				}
			}
			if (_hair.editorAsset == null)
			{
				var materialPath = FindMaterialPath("M_Hair_Transparent_Alpha_Clipping");
				if (materialPath != null)
				{
					var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
					_hair.SetEditorAsset(material);
				}
			}
			if (_skin.editorAsset == null)
			{
				var materialPath = FindMaterialPath("M_Skin_Opaque");
				if (materialPath != null)
				{
					var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
					_skin.SetEditorAsset(material);
				}
			}
			if (_skin02.editorAsset == null)
			{
				var materialPath = FindMaterialPath("M_Skin02_Opaque");
				if (materialPath != null)
				{
					var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
					_skin02.SetEditorAsset(material);
				}
			}
#endif
			if (MeshConfigs == null)
			{
				_meshConfigs = new List<MeshConfigEntry>();
			}

			if (_lodFalloffCurve == null || _lodFalloffCurve.length < 2)
			{
				_lodFalloffCurve = new AnimationCurve();
				_lodFalloffCurve.AddKey(0, 1);
				_lodFalloffCurve.AddKey(1, 0);
			}
		}
		
#if UNITY_EDITOR
		internal static SerializedObject GetSerializedSettings()
			=> new(GetOrCreateSettings());
		
		private static string FindMaterialPath(string materialName)
		{
			string searchFilter = $"t:Material {materialName}";
			var packagePath = UnityEditor.PackageManager.PackageInfo.FindForAssetPath($"Packages/com.futureverse.ubf")?.assetPath;
			packagePath ??= "Assets/Plugins/";
			
			var guids = AssetDatabase.FindAssets(searchFilter, new[] { packagePath });

			return guids.Length > 0 ? AssetDatabase.GUIDToAssetPath(guids[0]) : null;
		}
#endif
	}
}