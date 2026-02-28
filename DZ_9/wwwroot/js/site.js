// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Intersection Observer для анимации появления элементов
document.addEventListener('DOMContentLoaded', function () {

    // ===== Анимация титула для карточек =====
    const mainTitleObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('active');
                mainTitleObserver.unobserve(entry.target); // Отключаем после срабатывания
            }
        });
    }, {
        threshold: 0.1,              // Достаточно 10% видимости
        rootMargin: '0px 0px -50px 0px' // Менее агрессивный отступ
    });

    const mainTitle = document.querySelector('.philosophy-main-title');
    if (mainTitle) {
        mainTitleObserver.observe(mainTitle);
    }

    // ===== Анимация карточек =====
    const cardObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('active');
                cardObserver.unobserve(entry.target); // Отключаем после срабатывания
            }
        });
    }, {
        threshold: 0.1,              // Достаточно 10% видимости
        rootMargin: '0px 0px -50px 0px' // Менее агрессивный отступ
    });

    document.querySelectorAll('.reveal-left, .reveal-right').forEach(card => {
        cardObserver.observe(card);
    });

    // ===== Анимация заголовка для описания зон =====
    const titleObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
                titleObserver.unobserve(entry.target);
            }
        });
    }, {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    });

    const bottomTitle = document.querySelector('.zones-title');
    if (bottomTitle) {
        titleObserver.observe(bottomTitle);
    }
});
//console.log("✅ site.js загружен!");
//document.addEventListener('DOMContentLoaded', function () {

//    console.log("✅ DOM загружен, запускаем анимации...");

//    // Проверка: находит ли скрипт ваши элементы?
//    const cards = document.querySelectorAll('.reveal-left, .reveal-right');
//    console.log(`🔍 Найдено карточек для анимации: ${cards.length}`);

//    if (cards.length === 0) {
//        console.warn("⚠️ Внимание: Элементы с классами .reveal-left / .reveal-right не найдены в DOM!");
//    }



//    // ===== Анимация карточек (выезд слева/справа) =====
//    const cardObserver = new IntersectionObserver((entries) => {
//        entries.forEach(entry => {
//            if (entry.isIntersecting) {
//                entry.target.classList.add('active');
//                // Опционально: прекратить наблюдение после срабатывания
//                 cardObserver.unobserve(entry.target);
//            }
//        });
//    }, {
//        threshold: 0.2,           // Срабатывает, когда видно 20% элемента
//        rootMargin: '0px 0px -100px 0px' // Отступ снизу для раннего запуска
//    });

//    document.querySelectorAll('.reveal-left, .reveal-right').forEach(card => {
//        cardObserver.observe(card);
//    });

//    // ===== Анимация нижнего заголовка (появление снизу) =====
//    const titleObserver = new IntersectionObserver((entries) => {
//        entries.forEach(entry => {
//            if (entry.isIntersecting) {
//                entry.target.classList.add('visible');
//                // titleObserver.unobserve(entry.target);
//            }
//        });
//    }, {
//        threshold: 0.5,           // Срабатывает, когда видно 50% элемента
//        rootMargin: '0px 0px -150px 0px'
//    });

//    const bottomTitle = document.querySelector('.philosophy-bottom-title');
//    if (bottomTitle) {
//        titleObserver.observe(bottomTitle);
//    }
//});