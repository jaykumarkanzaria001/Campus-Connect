var data;
const API_URL = "https://campus-connect-msu.runasp.net/api/userAPI/signin";


// ====================== LOGIN ======================
document.getElementById("loginForm")?.addEventListener("submit", async (e) => {
    e.preventDefault();

    const user_name = document.getElementById("username").value;
    const password = document.getElementById("password").value;
    const role = document.getElementById("role").value;
    const errorEl = document.getElementById("error");

    try {
        const response = await fetch(API_URL, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ user_name, password, role })
        });

        if (!response.ok) {
            const msg = await response.text();
            throw new Error(msg);
        }

        data = await response.json();

        // STORE JWT TOKEN
        localStorage.setItem("jwtToken", data.token);
        localStorage.setItem("userRole", data.role);

        // REDIRECT BASED ON ROLE
        if (data.role === "Admin") {
            window.location.href = "index.html";
            errorEl.textContent = "Login sucessfully ";
        } else {
            window.location.href = "index.html";
        }

    } catch (err) {
        errorEl.textContent = err.message || "Login failed";
    }
});


function isAuthenticated() {
    const token = localStorage.getItem("jwtToken");
    if (!token) return false;

    // Optional: JWT expiry check
    const payload = JSON.parse(atob(token.split(".")[1]));
    const exp = payload.exp * 1000;

    if (Date.now() > exp) {
        logout();
        return false;
    }

    return true;
}


function protectPage() {
    if (!isAuthenticated()) {
        window.location.href = "Login.html";
    }
}


function logout() {
    localStorage.removeItem("jwtToken");
    localStorage.removeItem("userRole");
    window.location.href = "login.html";
}

// load footer
async function loadFooter() {
    const res = await fetch("footer.html");
    const data = await res.text();
    document.querySelector("footer").innerHTML = data;
}

loadFooter();