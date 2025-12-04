(() => {
  const themeToggle = document.getElementById("themeToggle");
  const themeIcon = document.getElementById("themeIcon");
  const languageSelect = document.getElementById("languageSelect");
  const sidebar = document.getElementById("sidebar");
  const sidebarToggle = document.getElementById("sidebarToggle");

  const applyTheme = (theme) => {
    document.body.setAttribute("data-theme", theme);
    if (themeIcon) {
      themeIcon.className = theme === "dark" ? "bi bi-moon-stars-fill" : "bi bi-sun-fill";
    }
    localStorage.setItem("portal-theme", theme);
  };

  const savedTheme = localStorage.getItem("portal-theme");
  if (savedTheme) {
    applyTheme(savedTheme);
  }

  themeToggle?.addEventListener("click", () => {
    const current = document.body.getAttribute("data-theme") === "dark" ? "dark" : "light";
    applyTheme(current === "dark" ? "light" : "dark");
  });

  languageSelect?.addEventListener("change", (event) => {
    localStorage.setItem("preferred-language", event.target.value);
  });

  sidebarToggle?.addEventListener("click", () => {
    sidebar?.classList.toggle("open");
  });
})();
