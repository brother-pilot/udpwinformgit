
namespace udpwinformgit
{
    public interface IView
    {

        //строка состояния
        string Status { get; set; }
        //вводимые пользователем данные
        string Mac { get; set; }
        string Ip { get; set; }
    }
}


