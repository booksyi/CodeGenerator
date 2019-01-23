RequestNode
    // 從 http post body
    {
        "From": 0 (HttpRequest.Body),
        "HttpRequestKey": "對應到 Http Body Key"
    }
    // 從 api 介接資料
    {
        "From": 1 (Adapter),
        "AdapterName": "介接命名",
        "AdapterPropertyName": "介接回傳資料的 Property Name (以逗號向下尋訪)"
    }
    // 套用其他樣板的結果
    {
        "From": 2 (Template),
        "TemplateUrl": "取得樣板的 Url (HttpGet)",
        "AdapterNodes": {
            “介接命名1” : AdapterNode,
            “介接命名2” : AdapterNode,
            “介接命名3” : AdapterNode
        },
        "TemplateRequestNodes": {
            "套用樣板索引1": RequestNode,
            "套用樣板索引2": RequestNode,
            "套用樣板索引3": RequestNode
        }
    }

AdapterNode
    {
        "Url": "介接資料的 Url (HttpPost)",
        "RequestNodes": {
            "給介接 Api 參數的名稱1": RequestNode,
            "給介接 Api 參數的名稱2": RequestNode,
            "給介接 Api 參數的名稱3": RequestNode
        },
        "Type": 0 (0: 回傳結果視為一個結果, 1: 回傳結果若為陣列則視為多個結果),
        "ResponseConfines": [
            "限縮回傳結果的 Property 1",
            "限縮回傳結果的 Property 2",
            "限縮回傳結果的 Property 3" (為 null 或空陣列則不限縮)
        ]
    }

GenerateNode
    {
        "applyKey": null,
        "applyValue": null,
        "applyApi": "http://localhost:4967/api/templates/1/context",
        "applyParameters": [
            GenerateNode,
            GenerateNode,
            GenerateNode
        ]
    }
    
1.整體功能與架構
2.先以功能面做一個小 Demo
3.介紹 Request 的 Class
    1.RequestNode
        1.格式說明
        2.AdapterPropertyName 向下尋訪
    2.AdapterNode
        1.格式說明
        2.Adapter 命名的作用
        3.Type 0 與 1 的差異
        4.ResponseConfines 限縮的作用
        5.多個 AdapterNode 的串接
4.調整 RequestNode 做一個小 Demo
5.介紹 Generate 的 Class
    1.GenerateNode
        1.格式說明
6.運用暫存的 Api
7.樣板的格式說明