using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : Interactable
{
	//Custom
	[SerializeField] private string speakerName;
	public string SpeakerName{ get{return speakerName;} set{speakerName = value;} }
	[SerializeField] private List<string> sentences;
	public List<string> Sentences{ get{return sentences;} set{sentences = value;} }
	[SerializeField] private float charRate;//Letters by second
	//Properties
	private int currentSentence;
	private int currentCharIndex;
	private float cooldown;

	//References
	[SerializeField] private Canvas canvasRef;
	[SerializeField] private Image npcImgRef;
	[SerializeField] private Text npcNameRef;
	[SerializeField] private Text dialogAreaRef;

	protected override void Initialization()
	{
		base.Initialization();
		currentSentence = 0;
		currentCharIndex = 0;
		cooldown = 0f;
	}

	private void Update()
	{
		WriteText();
	}

	public override void Interact() {
        if (beingUsed) {
            if(currentSentence < sentences.Count - 1 || (!SentenceOver())) {
                Skip();
            } else{
                Release();
            }
        } else {
            base.Interact();
            canvasRef.gameObject.SetActive(true);
            npcImgRef.GetComponent<Animator>().SetTrigger("jump");
        }
	}

	public override void Release()
	{
		base.Release();
		canvasRef.gameObject.SetActive(false);
        npcImgRef.GetComponent<Animator>().SetTrigger("jump");
        dialogAreaRef.text = "";
        currentSentence = 0;
        currentCharIndex = 0;
        cooldown = 0f;
    }

	private void WriteText()
	{
		if(CooldownUp())
		{
			if(beingUsed)
			{
				if(!SentenceOver())
				{
					cooldown = 1f/charRate;
					currentCharIndex ++;
					dialogAreaRef.text += sentences[currentSentence].ToCharArray()[currentCharIndex-1];
				}else
				{
					NewSentence();
				}
			}
		}else
		{
			cooldown -= Time.deltaTime;
		}
	}

	private void NewSentence()
	{
		if(currentSentence < sentences.Count - 1)
		{
			if(dialogAreaRef.text.ToCharArray()[dialogAreaRef.text.Length-1] != '\n')
				dialogAreaRef.text += '\n';
			currentCharIndex = 0;
			cooldown = 10f/charRate;
			currentSentence ++;
		}else
		{
			currentCharIndex = sentences[currentSentence].Length-1;
		}
	}

	private bool CooldownUp()
	{
		return(cooldown <= 0f);
	}

	public void Skip()
	{
		dialogAreaRef.text = "";
		for(int i = 0 ; i <= currentSentence ; i ++)
		{
			dialogAreaRef.text += sentences[i];
			dialogAreaRef.text += '\n';
		}
		NewSentence();
	}

	private bool SentenceOver()
	{
		return(currentCharIndex == sentences[currentSentence].Length - 1);
	}
}
