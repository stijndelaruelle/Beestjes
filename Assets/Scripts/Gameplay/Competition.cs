using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Competition : MonoBehaviour
{
    private Picture m_Picture;
    public Picture Picture
    {
        get { return m_Picture; }
    }

    public event PictureDelegate CompetitionPictureChangedEvent;

    //Rewards
    public void SetPicture(Picture picture)
    {
        m_Picture = picture;

        if (CompetitionPictureChangedEvent != null)
            CompetitionPictureChangedEvent(picture);

        SaveGameManager.Instance.SerializeCompetition();
    }

    public void Serialize(JSONNode rootNode)
    {
        JSONClass pictureNode = new JSONClass();
        m_Picture.Serialize(pictureNode);

        rootNode.Add("picture", pictureNode);
    }

    public void Deserialize(JSONNode rootNode)
    {
        JSONClass pictureNode = rootNode["picture"].AsObject;

        Picture picture = new Picture();
        bool success = picture.Deserialize(pictureNode);

        if (success)
            m_Picture = picture;
    }
}
