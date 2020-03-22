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
      .required('Tournament description is required'),
  startDate: Yup.date()
      .required('Tournament start date is required'),
  tournamentType: Yup.number()
      .min(1, "Wrong type selected")
      .max(2, "Wrong type selected")
      .required('Tournament type is required'),
  contests: Yup.number()
      .required('Number of contests is required'),
  droppedContests: Yup.number()
      .lessThan(Yup.ref('contests'), "Dropped contests must be less than number of contests")
      .required('Number of dropped contests is required'),
  entrants: Yup.number()
      .when('droppedContests', (droppedContests, schema) => {
        return droppedContests > 0
            ? schema
                .moreThan(Yup.ref('droppedContests'), "Number of entrants must be higher than dropped contests")
            : schema
                .moreThan(1, "Number of entrants must be higher than 1")
                .required('Number of entrants is required');
      })
});
