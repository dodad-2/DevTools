using Il2CppInterop.Runtime;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using MelonLoader;

namespace DevTools;

public static class Resources // TODO rewrite this
{
    //internal static Dictionary<string, Texture2D>? bundleTextures;

    #region Variables
    private static Dictionary<string, Il2CppAssetBundle>? BundleHash { get; set; }
    #endregion

    #region Bundles
    /// <summary>
    /// Makes bundles available to other mods
    /// <param name="bundleName">Bundle Name</parameter>
    /// <param name="bundleKey">Used for GetBundle</parameter>
    /// </summary>
    public static bool RegisterBundle(MelonMod mod, string bundleName)
    {
        Assembly assembly = System.Reflection.Assembly.GetCallingAssembly();

        if (assembly == null)
        {
            Log.LogOutput($"RegisterBundle: Unable to get assembly", Log.LogLevel.Error);
            return false;
        }

        if (BundleHash == null)
            BundleHash = new Dictionary<string, Il2CppAssetBundle>();

        try
        {
            Il2CppAssetBundle? bundle = null;

            Log.LogOutput($"RegisterBundle: Loading '{bundleName}' from assembly '{assembly.FullName}'", Log.LogLevel.Debug);

            foreach (string fileName in assembly.GetManifestResourceNames())
                Log.LogOutput($"RegisterBundle: Resource name: {fileName}", Log.LogLevel.Debug);

            MemoryStream memoryStream;

            using (Stream stream = assembly.GetManifestResourceStream(bundleName))
            {
                memoryStream = new MemoryStream((int)stream.Length);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                memoryStream.Write(buffer, 0, buffer.Length);
            }

            bundle = Il2CppAssetBundleManager.LoadFromMemory(memoryStream.ToArray());

            if (bundle == null)
            {
                Log.LogOutput($"RegisterBundle: Unable to load '{bundleName}'.", Log.LogLevel.Warning);
            }
            else
            {
                BundleHash.Add(bundleName, bundle);
                Log.LogOutput($"RegisterBundle: Registered bundle '{bundleName}'", Log.LogLevel.Info);
            }
        }
        catch (Exception e)
        {
            Log.LogOutput($"Check bundle name: {e}", Log.LogLevel.Error);
            return false;
        }

        return true;
    }
    public static Il2CppAssetBundle? GetBundle(string key)
    {
        if (key == null || key.Length == 0)
        {
            Log.LogOutput($"GetBundle: key is null or empty");
            return null;
        }

        if (BundleHash == null)
        {
            Log.LogOutput($"GetBundle: No bundles loaded", Log.LogLevel.Warning);
            return null;
        }

        if (!BundleHash.ContainsKey(key))
        {
            Log.LogOutput($"GetBundle: Bundle '{key}' not found", Log.LogLevel.Info);
            return null;
        }

        return BundleHash[key];
    }
    #endregion

    #region Helpers
    public static Dictionary<string, Texture2D>? GetTexturesFromBundle(string name)
    {
        if (BundleHash == null)
        {
            Log.LogOutput($"No bundles loaded");
            return null;
        }

        if (!BundleHash.ContainsKey(name))
        {
            Log.LogOutput($"Bundle '' not found");
            return null;
        }

        return GetTexturesFromBundle(BundleHash[name]);
    }
    public static Dictionary<string, Texture2D>? GetTexturesFromBundle(Il2CppAssetBundle? bundle)
    {
        if (bundle == null)
        {
            Log.LogOutput($"GetTexturesFromBundle: Unable to load textures: bundle is null");
            return null;
        }

        var textures = bundle.LoadAllAssets<Texture2D>();

        if (textures == null || textures.Length == 0)
        {
            Log.LogOutput($"GetTexturesFromBundle: No textures loaded", Log.LogLevel.Debug);
            return null;
        }

        Dictionary<string, Texture2D>? bundleTextures = new();
        string uid;
        int id;

        foreach (var texture in textures)
        {
            uid = texture.name;
            id = 1;

            while (bundleTextures.ContainsKey(uid))
            {
                uid = $"{texture.name}{id}";
                id++;
            }

            bundleTextures.Add(uid, texture);

            Log.LogOutput($"GetTexturesFromBundle: Found texture '{uid}'");
        }

        return bundleTextures;
    }
    #endregion
}