﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureAlbumPanel : IVisible
{
    public void Start()
    {
        Hide();
    }

    private void OnDestroy()
    {
        Hide();
    }
}
