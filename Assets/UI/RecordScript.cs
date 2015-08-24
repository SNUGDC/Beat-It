using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Song {
	public string name;
	public Sprite sprite;
}

public class RecordScript : MonoBehaviour {

	public int index = 0;
	private int indexMax;
	public List<Song> songs;
	private Animator animator;
	public Text songText;
	void Start()
	{
		animator = GetComponent<Animator>();
		indexMax = songs.Count - 1;
		GetComponent<Image>().sprite = songs[index].sprite;
		songText.text = songs[index].name;
	}
	void Update()
	{
		if(Input.GetKeyDown (KeyCode.S) || Input.GetKeyDown (KeyCode.K)) {
			index = (index == indexMax)? 0 : index + 1;
			animator.SetTrigger ("recordChanged");
			GetComponent<Image>().sprite = songs[index].sprite;
			songText.text = songs[index].name;
		}
	}
	/*IEnumerator UpdateSprite()
	{
		// wait until the end of the animation
		var animationState = animator.GetCurrentAnimatorStateInfo(0);
		var animationClips = animator.GetCurrentAnimatorClipInfo(0);
		var animationClip = animationClips[0].clip;
		var animationTime = animationClip.length * animationState.normalizedTime;
		yield return new WaitForSeconds(animationTime);
		isUpdated = true;
	}*/
	void LateUpdate()
	{
		GetComponent<Image>().sprite = songs[index].sprite;
	}
}
