import * as Yup from 'yup';

export const blogValidation = Yup.object().shape({
  title: Yup.string()
    .required('Title is required'),
  body: Yup.string()
    .required('Body is required')
});

export const newTournamentValidation = Yup.object().shape({
  tournamentIcon: Yup.string()
      .required('Tournament avatar is required'),
  tournamentTitle: Yup.string()
      .required('Tournament title is required'),
  tournamentDescription: Yup.string()
      .required('Tournament description is required')
});
