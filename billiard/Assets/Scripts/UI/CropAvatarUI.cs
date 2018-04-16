using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CropAvatarUI : MonoBehaviour
{
    public Image avatar;
    public Image sourceImage;
    public Image mask;
    public Texture2D result;

    public Texture2D MaskTexture;
    public Texture2D SourceTexture;
    int shapeId = 0;

    public void ClosePopup()
    {
        AudioController.Play("Button");        
        /*Destroy(gameObject);*/
        gameObject.SetActive(false);
    }
    public void OnUseBtn()
    {
        AudioController.Play("Button");
        ProfileScene.instance.OnCropedAvatar(result);        
    }

    public void OnImage(Texture2D image)
    {//after call Get image from camera 
        SourceTexture = image;
        sourceImage.sprite = CreateSprite(SourceTexture);
        sourceImage.SetNativeSize();
    }

    public void ApplyMaskAndroid()
    {
        //result = new Texture2D(result.width, result.height, TextureFormat.ARGB32, false);
        //TempTexture.filterMode = FilterMode.Point;
        //Copy pixels and scale
        Color EmptyColor = new Color();
        Color Pixel = Color.black;
        EmptyColor.a = 0;
        //		print (SourceTexture.width+":"+SourceTexture.height+":"+MaskTexture[shapeId].width);
        //		int MaskX0 = (int)((3*MaskTexture[shapeId].width/SourceTexture.width));
        //		print (MaskX0);
        //		MaskX0 = (int)((100*MaskTexture[shapeId].width/SourceTexture.width));

        int x00 = (int)mask.GetComponent<RectTransform>().anchoredPosition.x;
        int y00 = (int)mask.GetComponent<RectTransform>().anchoredPosition.y;        
        
        for (int X = 0; X < result.width; X++)
        {
            for (int Y = 0; Y < result.height; Y++)
            {
//                 int MaskX = (int)((X * MaskTexture[shapeId].width / SourceTexture.width));
//                 int MaskY = (int)((Y * MaskTexture[shapeId].height / SourceTexture.height));
                int x0 = X + x00;
                int y0 = Y + y00;
                Color MaskPixel = MaskTexture.GetPixel(X, Y);
/*                print(MaskPixel.a);*/
                if (MaskPixel.a < 0.2f && 0 <= x0 && x0 < SourceTexture.width && 0 <= y0 && y0 < SourceTexture.height)
                {
                    Pixel = SourceTexture.GetPixel(x0, y0);
                    result.SetPixel(X, Y, Pixel);
                }                
                else
                {
                    result.SetPixel(X, Y, EmptyColor);
                }
            }
        }
        result.Apply();
        avatar.sprite = CreateSprite(result);
    }
    
    public Sprite CreateSprite(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
}
