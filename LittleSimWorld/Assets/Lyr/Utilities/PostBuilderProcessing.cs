﻿#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class BuildPostProcessor {

	[PostProcessBuild(1)]
	public static void CopyGridData(BuildTarget target, string pathToBuiltProject) {

		Debug.Log("Copying Pathfinding Data on the project's directory.");

		var projectPath = Directory.GetParent(pathToBuiltProject);
		string copyFromPath = Application.dataPath + @"\GridData\";
		string targetDirPath = projectPath + @"\LittleSimWorld_Data\GridData";

		DirectoryCopy(copyFromPath, targetDirPath, false);

		Debug.Log($"Success.. Pathfinding Data is now on {targetDirPath}");
	}

	static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs) {
		// Get the subdirectories for the specified directory.
		DirectoryInfo dir = new DirectoryInfo(sourceDirName);

		if (!dir.Exists) { throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName); }

		DirectoryInfo[] dirs = dir.GetDirectories();
		// If the destination directory doesn't exist, create it.
		if (!Directory.Exists(destDirName)) { Directory.CreateDirectory(destDirName); }

		// Get the files in the directory and copy them to the new location.
		FileInfo[] files = dir.GetFiles();
		foreach (FileInfo file in files) {
			string temppath = Path.Combine(destDirName, file.Name);
			file.CopyTo(temppath, false);
		}

		// If copying subdirectories, copy them and their contents to new location.
		if (copySubDirs) {
			foreach (DirectoryInfo subdir in dirs) {
				string temppath = Path.Combine(destDirName, subdir.Name);
				DirectoryCopy(subdir.FullName, temppath, copySubDirs);
			}
		}
	}
}
#endif