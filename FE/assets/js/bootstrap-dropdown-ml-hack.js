! function(n) {
    const r = "has-child-dropdown-show";
    var e;
    n.Dropdown.prototype.toggle = (e = n.Dropdown.prototype.toggle, function() {
        document.querySelectorAll("." + r).forEach(function(o) {
            o.classList.remove(r)
        });
        let o = this._element.closest(".dropdown").parentNode.closest(".dropdown");
        for (; o && o !== document; o = o.parentNode.closest(".dropdown")) o.classList.add(r);
        return e.call(this)
    }), document.querySelectorAll(".dropdown").forEach(function(o) {
        o.addEventListener("hide.bs.dropdown", function(o) {
            this.classList.contains(r) && (this.classList.remove(r), o.preventDefault()), o.stopPropagation()
        })
    }), document.querySelectorAll(".dropdown-hover, .dropdown-hover-all .dropdown").forEach(function(t) {
        t.addEventListener("mouseenter", function(o) {
            let e = o.target.querySelector(':scope>[data-bs-toggle="dropdown"]');
            e.classList.contains("show") || (n.Dropdown.getOrCreateInstance(e).toggle(), t.classList.add(r), n.Dropdown.clearMenus())
        }), t.addEventListener("mouseleave", function(o) {
            let e = o.target.querySelector(':scope>[data-bs-toggle="dropdown"]');
            e.classList.contains("show") && n.Dropdown.getOrCreateInstance(e).toggle()
        })
    })
}(bootstrap);