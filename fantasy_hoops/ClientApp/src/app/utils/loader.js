export const addLoader = () => {
  const loader = document.body.getElementsByClassName('Loader')[0];
  if (loader.classList.contains('Loader--hidden')) {
    return;
  }

  loader.classList.remove('Loader--hidden');
};

export const removeLoader = () => {
  const loader = document.body.getElementsByClassName('Loader')[0];
  if (!loader.classList.contains('Loader--hidden')) {
    return;
  }

  loader.classList.add('Loader--hidden');
};
