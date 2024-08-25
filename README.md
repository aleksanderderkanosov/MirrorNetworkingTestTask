# MirrorNetworkingTestTask
## ТЗ
Создать сетевое приложение на платформе Unity, которое имитирует механику мяча и его взаимодействия с окружающим миром. Приложение должно включать в себя следующие функциональности:

- ✅ ***Мяч***: Разработать модель мяча с физическими свойствами, схожими с реальным футбольным мячом. Мяч, после выстрела из пушки на поле должен находиться некоторое время. Время вычислить следующим образом, чтобы на сцене не было очень много мячей, которые уже не дают вылететь другим мячам, и чтобы не падала производительность.

- ✅ ***Пушка***: Реализовать управление вращения пушкой с помощью мыши. Сила выстрела мяча из пушки, должна зависеть от длительности удержания кнопки мыши.

- ✅ ***Взаимодействие мяча со стенами***: После выстрела, мяч должен взаимодействовать со стенами, полом и потолком. При этом учесть физические параметры мяча, его перемещение и взаимодействие с окружением.

- ✅ ***Ворота***: Сделать небольшой куб, в виде ворот, который будет двигаться вдоль каждой стены подключенного игрока. Куб должен быть в цвет пушки. При попадании любым мячом в эти ворота у игрока, который над этими воротами убавляются очки. У игрока, который забил мяч в ворота, прибавляются очки. Итоговую таблицу вывода очков делать не нужно. Просто бесконечная перестрелка мячами.

- ✅ ***Стены и окружение***: Создать виртуальное окружение, которое будет состоять из стен со всех четырех сторон, а также пола и потолка.

- ✅ ***Физика***: Протестировать и настроить физическую модель мяча, чтобы достичь схожей реалистичности в его поведении при взаимодействии с окружением.

- ❌ ***Лобби и выбор цвета***: Реализовать систему лобби, где игроки могут присоединиться к игре или создать новую. В лобби игроки должны иметь возможность выбора цвета своей пушки (доступно 4 цвета).
Всего могут играть 4 игрока с каждой из сторон игрового поля.


***Общие требования***:
  Оптимизация производительности: Предпримите необходимые меры для обеспечения плавной работы приложения.
  Приложение должно демонстрировать реалистичное взаимодействие мяча с окружением и его поведение в соответствии с физическими законами. И сетевое взаимодействие игроков.

## Почему не сделан пункт ***Лобби и выбор цвета***
В какой-то момент перестала работать связка Mirror с Edgegap, через которую реализуется система лобби из коробки. 

Странно то, что когда я начал делать этот проект (20.08), я как раз начал с самого сложного - настройки системы лобби. И она работала - до следующего дня (21.08), когда всё вдруг сломалось. 

Точнее, сломалось не всё - если отправлять запросы к Edgegap напрямую (я отправлял через Postman), то ответы приходили корректные. Однако в движке соединение бесконечно показывает "Connecting" и не может принять данные. Я пытался дебажить `EdgegapKcpServer` и его метод `RawReceiveFrom(...)`, в котором и происходят проблемы с сокетом, но никакого результата не добился. 

Также я пробовал и другие методы, в т.ч откаты к версии 20.08 и аппаратные способы, но ничего не вышло. Я обратился на сервера комьюнити Mirror и Edgegap, но пока что не получил ответа:
![image](https://github.com/user-attachments/assets/76656ed2-c19c-4519-9482-38c9817ddc58)

Поэтому, на данный момент, подключаться можно только через UI напрямую друг к другу, т.е. вводя ip и порт хоста. 
Функционал смены цвета пушки и ворот есть в проекте, но без системы лобби не используется.

## Управление игрой
Для подключения есть UI в левом верхнем углу. Нужно ввести соответствующий IP-адрес и порт хоста и нажать нужный режим подключения (клиент или хост): 

![image](https://github.com/user-attachments/assets/173b3735-2766-4f2f-910a-f2948a65be96)

Также можно запустить несколько инстансов игры на одном ПК и подключиться через `localhost`.

Пушка управляется с помощью движения мыши, вертикальное наведение - через колёсико. Выстрел - на ЛКМ.

## Билд игры
Можно скачать [здесь](https://github.com/aleksanderderkanosov/MirrorNetworkingTestTask/releases/tag/0.1.1).
