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
        .min(5, "Tournament title must be at least 5 characters long")
        .max(20, "Tournament title must be less than 20 characters long")
        .required('Tournament title is required'),
    tournamentDescription: Yup.string()
        .min(5, "Tournament description must be at least 5 characters long")
        .max(50, "Tournament description must be less than 50 characters long")
        .required('Tournament description is required'),
    startDate: Yup.date()
        .required('Tournament start date is required'),
    tournamentType: Yup.number()
        .min(0, "Wrong type selected")
        .max(1, "Wrong type selected")
        .required('Tournament type is required'),
    contests: Yup.number()
        .required('Number of contests is required'),
    droppedContests: Yup.number()
        .when(['tournamentType', 'contests'], (tournamentType, contests, schema) => {
            return tournamentType === 1
                ? schema
                : schema.lessThan(contests, "Dropped contests must be less than number of contests")
                    .required('Number of dropped contests is required')
        }),
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
