using KVS_API.Models;

namespace KVS_API.Controllers;

interface IKontoService
{
    IEnumerable<Konto> GetKontos();
    Konto GetKonto(string id);

    void CreateUser();


}