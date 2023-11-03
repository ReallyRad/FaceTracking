using UnityEngine;
using System;

namespace Calcatz.CookieCutter {
    public class SimpleSeparableBlur {

        Color tint = Color.black;
        float tinting = 0.4f;
        float blurSize = 3.0f;
        int passes = 8;

        Material blurMaterial;
        RenderTexture destTexture;

        public SimpleSeparableBlur(int width, int height) {
            blurMaterial = new Material(Shader.Find("Hidden/Blur"));
            blurMaterial.SetColor("_Tint", tint);
            blurMaterial.SetFloat("_Tinting", tinting);
            blurMaterial.SetFloat("_BlurSize", blurSize);

            destTexture = new RenderTexture(width, height, 0);
            destTexture.Create();
        }

        public Texture BlurTexture(Texture sourceTexture) {
            RenderTexture active = RenderTexture.active; // Save original RenderTexture so we can restore when we're done.

            try {
                RenderTexture tempA = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);
                RenderTexture tempB = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);

                for (int i = 0; i < passes; i++) {
                    if (i == 0) {
                        Graphics.Blit(sourceTexture, tempA, blurMaterial, 0);
                    }
                    else {
                        Graphics.Blit(tempB, tempA, blurMaterial, 0);
                    }
                    Graphics.Blit(tempA, tempB, blurMaterial, 1);
                }

                Graphics.Blit(tempB, destTexture, blurMaterial, 2);

                RenderTexture.ReleaseTemporary(tempA);
                RenderTexture.ReleaseTemporary(tempB);
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
            finally {
                RenderTexture.active = active; // Restore
            }

            return destTexture;
        }
    }
}