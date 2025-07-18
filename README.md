# Universal Blueprint Framework - Unity Interpreter

**A UBF Interpreter for Unity by [Futureverse](https://www.futureverse.com)**

This SDK provides a runtime interpreter for the **Universal Blueprint Framework (UBF)**, designed specifically for use in Unity-based applications. It allows developers to execute UBF Blueprints inside Unity scenes using a flexible, engine-agnostic system. The Interpreter is an essential part of the broader UBF ecosystem.

> For more information about the Universal Blueprint Framework, its authoring tools, and the full ecosystem, please visit the [UBF Open Standard](https://ubfstandard.com/) and the [Futureverse Developer Documentation](https://docs.futureverse.com/1134b651-6817-4acb-ab1a-7bced4b15e80).

## Installation

Go to the Unity Package Manager window, and select **Add package from git URL...** and enter the following link:
https://github.com/futureversecom/sdk-unity-ubf.git?path=Assets/Plugins/UBF

To specify a version, append `#vX.X.X` (e.g., `#v1.2.3`).  
Alternatively, you can download a `.unitypackage` from the [Releases](https://github.com/futureversecom/sdk-unity-ubf/releases) page.

## Overview

The Unity UBF Interpreter converts Blueprint’s engine‑agnostic instructions — defined in compliance with the [UBF Open Standard](https://ubfstandard.com/) — into native API calls, commands, and data formats that Unity can understand and execute. This Interpreter sits on the execution side of the UBF stack. It is completely decoupled from authoring (UBF Studio) and orchestration (Execution Controller). For more on those, see [Creating UBF Content](https://docs.futureverse.com/1134b651-6817-4acb-ab1a-7bced4b15e80/ubf-studio-and-ubf-projects) and the [Unity Execution Controller](https://github.com/futureversecom/sdk-unity-execution-controller).

The Interpreter is intentionally narrow in responsibility: it takes a compiled UBF Blueprint as input and simply executes it within a Unity scene, without any awareness of the Blueprint’s intent or the broader system context. They are entirely blind to external concerns—such as equipment trees, blockchain concepts, or orchestration logic—which are handled elsewhere. This isolation makes the Interpreter highly generic, allowing it to stand alone and serve an open-ended range of use cases across various applications.
​
## Core Responsibilities

While the Interpreter operate within a narrow and clearly defined scope, its role in the UBF Framework is both foundational and critical. It fulfills several core responsibilities, including but not limited to:

* Deserializing UBF Artifacts: Ensuring that the data structures are well-formed, valid, and ready for use.
* Implementing Node Translations: Providing behavior (C# scripts) for each UBF node, in accordance with the standard specification.
* Managing Execution Flow: Handling the logical progression from one node to the next within a Blueprint’s execution, ensuring that dependencies and sequence are respected.
* Managing Data Flow Between Nodes: Coordinating how data is passed and transformed from one node to another.

The Interpreter is not responsible for:

* Resolving or downloading referenced artifacts
* Deciding when or where a blueprint should execute
* Determining what inputs are fed into a blueprint
* Mapping blueprints or catalogs to specific assets or their variants

These concerns fall outside the Interpreter’s scope and can be handled by the [Unity Execution Controller](https://github.com/futureversecom/sdk-unity-execution-controller).

## Using The Interpreter

Below is a small sample script to show how you can use the UBF Interpreter in your project. If you are unfamiliar with the terms Blueprint and Catalog, it is recommended to read the [UBF Standard](https://ubfstandard.com/) first and then return to this.

```csharp
public class SimpleExecutor : MonoBehaviour
{
  [SerializeField] private string _catalogUri;
  [SerializeField] private string _blueprintId;

  public IEnumerator Start()
  {
    // Load the catalog via the CatalogResourceLoader
    Catalog catalog = null;
    var catalogLoader = new CatalogResourceLoader(_catalogUri);
    yield return catalogLoader.Get(c => catalog = c);
			
    // Make sure to break if the catalog did not download correctly
    if (catalog == null)
    {
      yield break;
    }

    // Register the catalog to the Artifact Provider singleton
    ArtifactProvider.Instance.RegisterCatalog(catalog);

    // Create a new BlueprintInstanceData using the serialized Blueprint ID, and add it to a new list
    var blueprintDefinition = new BlueprintInstanceData(_blueprintId);
    var blueprints = new List<IBlueprintInstanceData>
    {
      blueprintDefinition
    };

    // Create the ExecutionData using the data we established
    var executionData = new ExecutionData(
      transform,
      _ => Debug.Log("Completed"),
      blueprints
    );

    // Run the UBF Blueprint using the UBFExecutor
    yield return UBFExecutor.ExecuteRoutine(executionData, blueprintDefinition.InstanceId);
  }
}
```

This script assumes you have a URL for a Catalog that contains a Blueprint Resource. This is just to demonstrate how you can prepare the required data and use UBFExecutor to run the UBF system, but realistically you will use the Execution Controller to obtain the Catalog URLs.

Alternatively, you can check out the `SumOfTwoNumbers` Sample that is included with the SDK. It shows how you can run a Blueprint that is stored locally in your Unity project.

