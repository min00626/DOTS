using UnityEngine;

public class RandomIntegers : MonoBehaviour
{
	public static int[] GetUniqueRandomNumbers(int count, int minValue, int maxValue)
	{
		int[] array = new int[maxValue - minValue + 1];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = minValue + i;
		}

		for (int i = array.Length - 1; i > 0; i--)
		{
			int randomIndex = Random.Range(0, i + 1);
			int temp = array[i];
			array[i] = array[randomIndex];
			array[randomIndex] = temp;
		}

		int[] result = new int[count];
		for (int i = 0; i < count; i++)
		{
			result[i] = array[i];
		}

		return result;
	}
}
