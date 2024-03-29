import React from 'react';
import Img from 'react-image';
import defaultPhoto from '../../../../content/images/default.png';
import Routes from '../../../routes/routes';

export const RequestCard = (props) => {
  const {
    id, userName, type, cancel, decline, accept, avatarURL
  } = props;

  return (
    <div className="card rounded mx-auto RequestCard">
      <div className="RequestCard__Body">
        <a href={`${Routes.PROFILE}/${userName}`} className="card-body RequestCard__Info">
          <Img
            className="RequestCard__Avatar Avatar--round"
            alt={userName}
            src={[
              `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${avatarURL}.png`,
              defaultPhoto
            ]}
            decode={false}
          />
          <p className="RequestCard__Info__Name">{userName}</p>
        </a>
        <div className="RequestCard__Buttons">
          {
            type === 1
              ? (
                <button type="button" onClick={() => cancel(id)} className="btn btn-outline-danger">Cancel Request</button>
              )
              : (
                <div className="RequestCard__Buttons__Pending">
                  <button type="button" onClick={() => decline(id)} className="btn btn-outline-danger">Decline Request</button>
                  <button type="button" onClick={() => accept(id)} className="btn btn-info RequestCard__Buttons__Pending__Accept">Accept Request</button>
                </div>
              )
          }
        </div>
      </div>
    </div>
  );
};

export default RequestCard;
