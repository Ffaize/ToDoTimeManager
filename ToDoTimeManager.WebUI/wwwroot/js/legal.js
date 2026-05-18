function initLegalScroll() {
    document.querySelectorAll('.legal-toc a').forEach(function (a) {
        a.addEventListener('click', function (e) {
            e.preventDefault();
            var id = a.getAttribute('href').replace('#', '');
            var target = document.getElementById(id);
            if (target) target.scrollIntoView({ behavior: 'smooth', block: 'start' });
        });
    });
}
