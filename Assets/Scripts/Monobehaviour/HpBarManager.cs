using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarManager : MonoBehaviour
{
	public static HpBarManager instance;

	[SerializeField] GameObject hpBarPrefab;
	[SerializeField] GameObject hpBarHolder;

	private Queue<Image> hpBarPool = new Queue<Image>();

	private void Awake()
	{
		instance = this;
	}

	public Image GetHpBarInstance()
	{
		Image image;
		if(hpBarPool.Count > 0)
		{
			image = hpBarPool.Dequeue();
		}
		else
		{
			image = Instantiate(hpBarPrefab, hpBarHolder.transform).GetComponent<Image>();
		}
		image.enabled = true;
		return image;
	}

	public void DisposeHpBarInstance(Image image)
	{
		image.enabled = false;
		hpBarPool.Enqueue(image);
	}
}
