export const loadImage = (src, fallBack) => {
    return new Promise((resolve) => {
        let img = new Image()
        img.onload = () => resolve(img.src)
        img.onerror = () => resolve(fallBack)
        img.src = src
    })
}