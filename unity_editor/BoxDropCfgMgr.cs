using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class BoxDropCfgMgr {
    Dictionary<int, List<BoxDropVo>> boxDataList;
    void findBoxInfo()
    {
        boxDataList = new Dictionary<int, List<BoxDropVo>>();
        foreach (var item in m_ItemTable.Values)
        {
            if (boxDataList.ContainsKey(item.BoxId))
            {
                var _boxData = boxDataList[item.BoxId];
                if (_boxData == null)
                {
                    _boxData = new List<BoxDropVo>();
                }
                _boxData.Add(item);
            }
            else
            {
                boxDataList.Add(item.BoxId, new List<BoxDropVo>() { item });
            }
        }
    }

    public List<BoxDropVo> GetBoxData(int boxId)
    {
        if (boxDataList == null)
            findBoxInfo();
        if (boxDataList.ContainsKey(boxId))
            return boxDataList[boxId];
        return null;
    }

    public Dictionary<int, List<BoxDropVo>> BoxDataList
    {
        get
        {
            if (boxDataList == null || boxDataList.Count == 0)
                findBoxInfo();
            return boxDataList;
        }
    }
}
