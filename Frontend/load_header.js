async function loadHeader() {
    const res = await fetch("header.html");
    const data = await res.text();
    document.querySelector("header").innerHTML = data;
}

loadHeader();

function showmenu() {
    document.querySelector("#mobile-nav-section").innerHTML = `<ul>
                    <li><a href="index.html" class="link">Home</a></li>
                    <li><a href="lost & found.html" class="link">Lost & Found</a></li>
                    <li><a href="Buy & sell.html" class="link">Buy & Sell</a></li>
                    <li><a href="Complaint & Suggestion.html" class="link">Complaint</a></li>
                    <li><a href="Login.html" login="" class="btn">Sign Out</a></li>
                </ul>`;
}