# ArticleMaster
1. Запустить консольное приложение ArticleMaster.Scraper. Произойдет парсинг сайта с последующим сохранением данных в БД.
2. Запустить ArticleMaster.API.
  Доступно три эндпоинта: 
    1. /api/posts?from=&to  Вернётся список статей

       ```
        {
          "id": "<Guid>",
          "datePublished": "<DateTime>",
          "downloadedFrom": "<string>",
          "title": "<string>",
          "content": "<string>",
          "author": {
            "id": "<Guid>",
            "authorName": "<string>"
          }
        }
        ```
    3. /api/topten/  Вернется 10 самых часто используемых слов в статьях
    
        ```
          [string]
        ```
    
    4. /api/search?text=asd Вернутся статьи в которых встречается текст
    
        ```
        {
          "id": "<Guid>",
          "datePublished": "<DateTime>",
          "downloadedFrom": "<string>",
          "title": "<string>",
          "content": "<string>",
          "author": {
            "id": "<Guid>",
            "authorName": "<string>"
          }
        }
    ```
