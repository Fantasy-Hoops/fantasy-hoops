import React from 'react';
import PropTypes from 'prop-types';
import moment from 'moment';
import Img from 'react-image';
import {UTCNow} from '../../utils/date';
import defaultLogo from '../../../content/images/defaultLogo.png';
import HistoryIcon from '@material-ui/icons/History';

import './InjuryCard.css';

const InjuryCard = (props) => {
    const {injury, animated, handleOpenDialog} = props;
    let status = '';
    if (injury.status.toLowerCase().includes('active')) {
        status = 'injury-active';
    } else if (
        injury.status.toLowerCase().includes('out')
        || injury.status.toLowerCase().includes('injured')
        || injury.status.toLowerCase().includes('suspended')) {
        status = 'injury-out';
    } else status = 'injury-questionable';
    const injuryDateUTC = new Date(injury.date).getTime();

    function handleDialogOpen(injury) {
        handleOpenDialog(injury);
    }

    const teamLogo = (
        <Img
            width="60%"
            className="InjuryCard__TeamLogo--behind"
            alt={injury.player.team.abbreviation}
            src={[`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/logos/${injury.player.team.abbreviation}.svg`,
                defaultLogo]}
            decode={false}
        />
    );
    return (
        <div className={`InjuryCardContainer inactive ${animated}`} onClick={() => handleDialogOpen(injury)}>
            <div className="InjuryCard">
                <div className="InjuryCard__PlayerImage--background">
                    <canvas
                        className="PlayerCard__background"
                        style={{backgroundColor: `${injury.player.team.color}`}}
                        width="260"
                        height="190"
                    />
                    <div className="InjuryCard__PlayerPosition badge">
                        {injury.player.position}
                    </div>
                    {teamLogo}
                    <Img
                        className="InjuryCard__PlayerImage"
                        width="100%"
                        alt={injury.player.fullName}
                        src={
                            [
                                `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/players/${injury.player.nbaID}.png`,
                                require(`../../../content/images/positions/${injury.player.position.toLowerCase()}.png`)
                            ]
                        }
                        loader={(
                            <img
                                className="InjuryCard__loader"
                                src={require('../../../content/images/imageLoader2.gif')}
                                alt="Loader"
                            />
                        )}
                        decode={false}
                    />
                    <div className={`InjuryCard__StatusBadge ${status}`}>
                        <span style={{opacity: 0.9}}>{injury.status}</span>
                    </div>
                </div>
                <div className="info">
                    <div className="InjuryCard__PlayerName">{injury.player.abbrName}</div>
                    <div className="InjuryCard__Title">{injury.injury}</div>
                </div>
                <div className="InjuryCard__Datetime">
                    <HistoryIcon fontSize={"small"}/>
                    {' '}
                    {moment(injuryDateUTC).from(UTCNow().Time)}
                </div>
            </div>
        </div>
    );
};

InjuryCard.propTypes = {
    injury: PropTypes.shape({
        status: PropTypes.string.isRequired,
        title: PropTypes.string.isRequired,
        description: PropTypes.string,
        date: PropTypes.string.isRequired,
        player: PropTypes.shape({
            nbaID: PropTypes.number.isRequired,
            position: PropTypes.string.isRequired,
            fullName: PropTypes.string.isRequired,
            abbrName: PropTypes.string.isRequired,
            team: PropTypes.shape({
                abbreviation: PropTypes.string.isRequired,
                color: PropTypes.string.isRequired
            }).isRequired
        }).isRequired
    }).isRequired,
    animated: PropTypes.string
};

InjuryCard.defaultProps = {
    animated: ''
};

export default InjuryCard;
