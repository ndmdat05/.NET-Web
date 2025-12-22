window.onload = function () {
    const links = document.querySelectorAll(".tab-link");
    const sections = document.querySelectorAll(".section");

    links.forEach(link => {
        link.onclick = function (e) {
            e.preventDefault();

            links.forEach(l => l.classList.remove("active"));
            this.classList.add("active");

            sections.forEach(sec => sec.classList.remove("active"));
            const target = this.getAttribute("href").substring(1);
            document.getElementById(target).classList.add("active");
        };
    });
};

