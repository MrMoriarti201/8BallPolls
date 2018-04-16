using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class CCEditText : MonoBehaviour{
    private List<CardType> mTypes;
    private Type currentType = Type.WRONG;
    private string mask;
	string text;

	void Awake()
	{
		GetComponent<InputField>().onValueChange.AddListener(delegate { Correct(); }); 
	}

    public void Correct() {
		text = transform.GetComponent<InputField>().text;
        chooseMask(text);
    }

    public void updateMask(string mask)
    {
        this.mask = mask;
		List<int> splitMask = new List<int>();
		string maskCopy = mask;
		if (mask!=null)
		{
		while (maskCopy.LastIndexOf(' ') != -1)
		{
			int temp = maskCopy.LastIndexOf(' ');
			maskCopy = maskCopy.Remove(temp, 1);
			splitMask.Add(temp);
		}

			text = text.Replace(" ", "");
			for (int i=splitMask.Count-1; i>-1; i--)
			{
				if (splitMask[i]<text.Length && text[splitMask[i]]!=' ')
					text = text.Insert(splitMask[i]," ");
				else
					break;
			}
			transform.GetComponent<InputField>().text = text;
			transform.GetComponent<InputField>().MoveTextEnd(false);
		}
    }

    private void chooseMask(string s) {
        currentType = getType(s);
        switch (currentType) {
            case Type.AMEX:
            case Type.ENROUTE:
            case Type.JSB15:
                updateMask("#### ###### #####");
                break;
            case Type.DINNERS_CLUB_CARTE_BLANCHE:
            case Type.DINERS_CLUB_INTERNATIONAL:
                updateMask("#### #### #### ##");
                break;
            case Type.VISA:
            case Type.VISA_ELECTRON:
            case Type.MASTERCARD:
            case Type.MAESTRO:
            case Type.JSB16:
                updateMask("#### #### #### ####");
                break;
            default:
                updateMask("#### #### #### #### ###");
                break;
        }
    }

    public enum Type {
        AMEX, ENROUTE, JSB15, DINNERS_CLUB_CARTE_BLANCHE,
        DINERS_CLUB_INTERNATIONAL, VISA, VISA_ELECTRON,
        MASTERCARD, DISCOVER, JSB16, MAESTRO, LASER, WRONG
    }

    public class CardType {

        public Type type;
        public Regex pattern;
        public int[] length;

        public CardType(Type mType, Regex pattern, int[] length)
        {
            this.type = mType;
            this.pattern = pattern;
            this.length = length;
        }
    }

	public bool IsMaestro()
	{
		return getCurrentType () == Type.MAESTRO;
	}

    private List<CardType> getTypes() {
        if (mTypes == null) {
            mTypes = new List<CardType>();
            mTypes.Add(new CardType(Type.AMEX, new Regex("^3[47]"), new int[]{15}));
            mTypes.Add(new CardType(Type.DINNERS_CLUB_CARTE_BLANCHE, new Regex("^30[0-5]"), new int[]{14}));
            mTypes.Add(new CardType(Type.DINERS_CLUB_INTERNATIONAL, new Regex("^36"), new int[]{14}));
            mTypes.Add(new CardType(Type.JSB16, new Regex("^3"), new int[]{16}));
            mTypes.Add(new CardType(Type.JSB15, new Regex("^(2131|1800)"), new int[]{16}));
            mTypes.Add(new CardType(Type.LASER, new Regex("^(6304|670[69]|6771)"), new int[]{16, 17, 18, 19}));
            mTypes.Add(new CardType(Type.VISA_ELECTRON, new Regex("^(4026|417500|4508|4844|491(3|7))"), new int[]{16}));
            mTypes.Add(new CardType(Type.VISA, new Regex("^4"), new int[]{16}));
            mTypes.Add(new CardType(Type.MASTERCARD, new Regex("^5[1-5]"), new int[]{16}));
            mTypes.Add(new CardType(Type.MAESTRO, new Regex("^(5018|5020|5038|6304|6759|676[1-3])"), new int[]{12, 13, 14, 15, 16, 17, 18, 19}));// CVV is not required
            mTypes.Add(new CardType(Type.DISCOVER, new Regex("^(6011|622(12[6-9]|1[3-9][0-9]|[2-8][0-9]{2}|9[0-1][0-9]|92[0-5]|64[4-9])|65)"), new int[]{16}));
            mTypes.Add(new CardType(Type.ENROUTE, new Regex("^(2(014|149))"), new int[]{15}));
        }
        return mTypes;
    }

    private Type getType(string cardNumber) {
        List<CardType> types = getTypes();
        MatchCollection m;
        foreach (CardType cardType in types) {
            m = cardType.pattern.Matches(cardNumber);
            if (m.Count > 0)
                return cardType.type;
        }
        return Type.WRONG;
    }

    public Type getCurrentType() {
        return currentType;
    }
}
