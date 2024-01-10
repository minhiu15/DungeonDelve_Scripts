using System.Collections.Generic;

/// <summary>
/// Danh sách Instance IGUI có trong danh sách GUIDatas để xử lí data game trên UI.
/// </summary>
public static class GUI_Manager
{
    private static readonly List<IGUI> GUIDatas = new();
    
    /// <summary>
    /// Gửi RefData tới các GUI có đăng kí nhận ref, chỉ gọi 1 lần duy nhất mỗi lần scene này được khởi tạo
    /// </summary>
    /// <param name="_gameManager"> Dữ liệu của user </param>
    public static void SendRef(GameManager _gameManager) => GUIDatas.ForEach(item => item.GetRef(_gameManager));

    
    /// <summary>
    /// Cập nhật lại tất cả dữ liệu trên toàn bộ UI 
    /// </summary>
    public static void UpdateGUIData() => GUIDatas.ForEach(gui => gui.UpdateData());
    
    public static void Add(IGUI iGui)
    {
        if(GUIDatas.Contains(iGui)) return;
        GUIDatas.Add(iGui);
    }
    public static void Remove(IGUI iGui)
    {
        if(!GUIDatas.Contains(iGui)) return;
        GUIDatas.Remove(iGui);
    }

}
