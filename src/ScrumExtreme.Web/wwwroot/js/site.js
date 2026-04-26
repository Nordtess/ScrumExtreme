// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification

document.addEventListener('DOMContentLoaded', function () {

    var track = document.querySelector('.carousel-track');
    if (track) {
        var cards = document.querySelectorAll('.carousel-card');
        var prevBtn = document.querySelector('.prev-btn');
        var nextBtn = document.querySelector('.next-btn');
        var currentIndex = 0;

        function updateCarousel() {
            var wrapper = document.querySelector('.carousel-track-wrapper');
            var wrapperWidth = wrapper.offsetWidth;
            var card = cards[0];
            var cardStyle = getComputedStyle(card);
            var cardSlotWidth = card.offsetWidth
                + parseFloat(cardStyle.marginLeft)
                + parseFloat(cardStyle.marginRight);
            var offset = (wrapperWidth / 2) - (cardSlotWidth / 2) - (currentIndex * cardSlotWidth);
            track.style.transform = 'translateX(' + offset + 'px)';
            cards.forEach(function (c, i) {
                c.classList.toggle('active', i === currentIndex);
            });
        }

        prevBtn.addEventListener('click', function () {
            currentIndex = (currentIndex - 1 + cards.length) % cards.length;
            updateCarousel();
        });
        nextBtn.addEventListener('click', function () {
            currentIndex = (currentIndex + 1) % cards.length;
            updateCarousel();
        });
        window.addEventListener('resize', updateCarousel);
        updateCarousel();
    }

    var elements = document.querySelectorAll('.scroll-animate');

    var observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                var el = entry.target;
                var delay = parseInt(el.dataset.delay) || 0;
                setTimeout(function () {
                    el.classList.add('visible');
                }, delay);
                observer.unobserve(el);
            }
        });
    }, { threshold: 0.2 });

    elements.forEach(function (el) {
        observer.observe(el);
    });

});
