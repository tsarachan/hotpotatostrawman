using Assets.Pixelation.Example.Scripts;
using System.Collections;
using UnityEngine;

namespace Assets.Pixelation.Scripts
{
    [ExecuteInEditMode]
    [AddComponentMenu("Image Effects/Color Adjustments/Pixelation")]
    public class Pixelation : ImageEffectBase
    {
        [Range(64.0f, 512.0f)] public float BlockCount = 128;
		private float currentBlockCount;
		private float setBlockCount;


		protected override void Start(){
			base.Start();

			currentBlockCount = BlockCount;
		}


        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            float k = Camera.main.aspect;
            Vector2 count = new Vector2(currentBlockCount, currentBlockCount/k);
            Vector2 size = new Vector2(1.0f/count.x, 1.0f/count.y);
            //
            material.SetVector("BlockCount", count);
            material.SetVector("BlockSize", size);
            Graphics.Blit(source, destination, material);
        }


		public void SetTemporaryResolution(float resolution, float changeDuration){
			setBlockCount = resolution;

			StartCoroutine(ResetDuration(changeDuration));
		}


		private IEnumerator ResetDuration(float duration){
			float timer = 0.0f;

			while (timer <= duration){
				timer += Time.deltaTime;

				currentBlockCount = Mathf.Lerp(setBlockCount, BlockCount, timer/duration);

				yield return null;
			}

			yield break;
		}
    }
}