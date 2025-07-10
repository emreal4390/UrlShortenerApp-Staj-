const path = require("path");

module.exports = {
    mode: "development",  // Geliştirme modu
    entry: "./Scripts/forgot-password.jsx",  // React dosyasının yolu
    output: {
        path: path.resolve(__dirname, "wwwroot/js"),  // Çıktı dosyasının konumu
        filename: "forgot-password.bundle.js",  // Çıktı dosyasının adı
    },
    module: {
        rules: [
            {
                test: /\.jsx?$/,  // JSX ve JS dosyalarını işler
                exclude: /node_modules/,  // node_modules klasörünü dışlar
                use: {
                    loader: "babel-loader",  // Babel ile JSX'i dönüştürür
                },
            },
        ],
    },
    resolve: {
        extensions: [".js", ".jsx"],  // Bu uzantıları çözümle
    },
};
