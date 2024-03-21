import React from 'react';
import CityForm from '../Components/CityForm';

function CityTable({url}) {
  return (
    <div>
      <CityForm url={url} />;
    </div>
  )
}

export default CityTable