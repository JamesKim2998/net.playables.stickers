using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions.Custom;
using UnityEngine;

public class PostBuild_AddStickers : IPostprocessBuildWithReport
{
	const string stickersPath = "Stickers"; // next to Assets
	
	//[MenuItem("Tools/Add Stickers to xcode project")]
	//public static void AddStickers()
	//{
	//	AddStickerExtension(stickersPath,"Builds/ios");
	//}


	public int callbackOrder => 0;

	public void OnPostprocessBuild(BuildReport report)
	{
		if (report.summary.platform == BuildTarget.iOS)
		{
			// if(StickerSettings.GetOrCreateSettings().exportEnabled)
				AddStickerExtension(stickersPath,report.summary.outputPath);
		}
	}
	
	public static void AddStickerExtension(string stickersPath, string pathToBuiltProject)
	{
		string pbxPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);

		PBXProject pbxProject = new PBXProject();
		pbxProject.ReadFromFile(pbxPath);

		if (pbxProject.TargetGuidByName("Stickers") != null)
			return;

		string appGuid = pbxProject.GetUnityMainTargetGuid();

		string stickersGuid = pbxProject.AddStickerExtension(appGuid, "Stickers",  PlayerSettings.applicationIdentifier+".Stickers", "Stickers/Info.plist");

		pbxProject.SetTeamId(stickersGuid,PlayerSettings.iOS.appleDeveloperTeamID);
		
		var stickersInfoGuid = pbxProject.AddFile("Stickers/Info.plist", "Stickers/Info.plist");

		var stickersAssetGuid = pbxProject.AddFile("Stickers/Stickers.xcassets", "Stickers/Stickers.xcassets");
		pbxProject.AddFileToBuild(stickersGuid, stickersAssetGuid);

		pbxProject.SetBuildProperty(stickersGuid, "IPHONEOS_DEPLOYMENT_TARGET", "16.2");
		pbxProject.AddBuildProperty(stickersGuid, "TARGETED_DEVICE_FAMILY", "1,2");
		pbxProject.AddBuildProperty(stickersGuid, "ASSETCATALOG_COMPILER_APPICON_NAME", "iMessage App Icon");
		pbxProject.AddBuildProperty(stickersGuid, "ALWAYS_SEARCH_USER_PATHS", "NO");
		pbxProject.AddBuildProperty(stickersGuid, "GENERATE_INFOPLIST_FILE", "YES");

		pbxProject.WriteToFile(pbxPath);
	}
}
