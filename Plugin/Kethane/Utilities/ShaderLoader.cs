﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using KSPAssets.Loaders;

namespace Kethane.ShaderLoader
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class KethaneShaderLoader: MonoBehaviour
    {
		const string gamedata = "GameData/Kethane/";
        static Dictionary<string, Shader> shaderDictionary = null;
        public static bool loaded = false;

        private void Start()
        {
            LoadShaders();
        }

        private void LoadShaders()
        {
			string kethane_bundle = "kethane-linux.ksp";

			switch (Environment.OSVersion.Platform) {
				case PlatformID.Win32NT:
					kethane_bundle = "kethane-windows.ksp";
					break;
				case PlatformID.MacOSX:
					kethane_bundle = "kethane-macosx.ksp";
					break;
				case PlatformID.Unix:
					kethane_bundle = "kethane-linux.ksp";
					break;
				default:
					Debug.LogFormat ("[Kethane] unknown platform: {0}", Environment.OSVersion.Platform);
					break;
			}

            if (shaderDictionary == null) {
                shaderDictionary = new Dictionary<string, Shader>();
				string root = KSPUtil.ApplicationRootPath;
				string url = "file://" + root + gamedata + kethane_bundle;

                using (WWW www = new WWW(url)) {
                    if (www.error != null) {
                        Debug.LogFormat("[Kethane] {0} not found!",
										kethane_bundle);
                        return;
                    }

                    AssetBundle bundle = www.assetBundle;

                    Shader[] shaders = bundle.LoadAllAssets<Shader>();

                    foreach (Shader shader in shaders) {
                        Debug.LogFormat ("[Kethane] Shader {0} loaded",
										 shader.name);
                        shaderDictionary[shader.name] = shader;
                    }

                    bundle.Unload(false);
                    www.Dispose();
                }

                loaded = true;
            }
        }

        public static Shader FindShader(string name)
        {
            if (shaderDictionary == null) {
                Debug.Log("[Kethane] Trying to find shader before assets loaded");
                return null;
            }
            if (shaderDictionary.ContainsKey(name))
            {
                return shaderDictionary[name];
            }
            KSPLog.print("[Kethane] Could not find shader " + name);
            return null;
        }
    }
}
