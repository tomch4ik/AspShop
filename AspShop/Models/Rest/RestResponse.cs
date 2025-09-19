namespace AspShop.Models.Rest
{
    public class RestResponse
    {
        public RestStatus Status { get; set; } = new();
        public RestMeta Meta { get; set; } = new();
        public Object? Data { get; set; }
    }
}
/*
 * REST (Representational State Transfer) — набір архітектурних вимог/принципів,
 * реалізація яких покращує роботу розподілених систем.
 *
 * API (Application Program Interface) — інтерфейс взаємодії Програми з своїми
 * Застосунками
 *
 * у цьому контексті Програма — інформаційний центр, зазвичай бекенд
 * Застосунок — самостійна частина, зазвичай клієнтського призначення,
 * яка взаємодіє з Програмою шляхом обміну даними.
 * (* також Додаток — несамостійна програма — плагін або розширення *)
*/


/* Приклад REST-відповіді
{
  "status": {
    "isOK": true,
    "code": 200,
    "phrase": "OK"
  },
/*
{
"meta": {
      "service": "Shop API",
      "serverTime": 1234987651,
      "url": "https://shop.site/api",
      "cache": 84600,
      "opt": {
        "page": 2,
        "lastPage": 10,
        "perPage": 20,
        "total": 193
      },
      "links": {
        "cart": "https://shop.site/api/cart",
        "signIn": "https://shop.site/api/user"
      },
     "dataType": "json/object"
},
  "data": 
    {
        "groups": [ ... ],
        "actions": [ ... ],
        "topSale": [ ... ]
    }
}
*/

